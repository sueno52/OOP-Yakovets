using System.Text;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        CustomExceptionDemo();
        await RetryScenario();
        await CircuitBreakerScenario();
        await TimeoutRetryFallbackScenario();
    }

    private static void CustomExceptionDemo()
    {
        Console.WriteLine("0. Власний виняток SeatAlreadyTakenException");
        var booking = new BookingService();

        foreach (var (row, seat) in new[] { (5, 7), (5, 7), (0, 3) })
        {
            try
            {
                Console.WriteLine(booking.BookSeat("Дюна", row, seat));
            }
            catch (SeatAlreadyTakenException ex) 
            {
                Console.WriteLine($"Помилка: {ex.Message} (Session={ex.Session}, Row={ex.Row}, Seat={ex.Seat})");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Невірні дані: {ex.Message}");
            }
            catch (Exception ex)                
            {
                Console.WriteLine($"Інша помилка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"   (спроба забронювати {row}-{seat} завершена)");
            }
        }
        Console.WriteLine();
    }

    private static async Task RetryScenario()
    {
        Console.WriteLine("1. Оплата квитка: Retry (exponential + jitter)");
        var gateway = new PaymentGateway(failuresBeforeSuccess: 2);

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(300),
                BackoffType = DelayBackoffType.Exponential, 
                UseJitter = true,                           
                OnRetry = args =>
                {
                    Log($"Retry #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс: " +
                        $"{args.Outcome.Exception?.Message}");
                    return default;
                }
            })
            .Build();

        try
        {
            string result = await pipeline.ExecuteAsync(
                async token => await gateway.ChargeAsync(250m, "order-1001", token));
            Log($"Успіх: {result}");
        }
        catch (HttpRequestException ex)
        {
            Log($"Остаточна невдача: {ex.Message}");
        }
        Console.WriteLine();
    }

    private static async Task CircuitBreakerScenario()
    {
        Console.WriteLine("2. Схема залу: Circuit Breaker");
        var seatMap = new SeatMapService(failuresBeforeRecovery: 3);

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                FailureRatio = 0.5,                        
                MinimumThroughput = 3,                     
                SamplingDuration = TimeSpan.FromSeconds(5),  
                BreakDuration = TimeSpan.FromSeconds(1),    
                OnOpened = args =>
                {
                    Log($"Вимикач РОЗІМКНЕНО (Open) на {args.BreakDuration.TotalSeconds} с");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    Log("Вимикач НАПІВРОЗІМКНЕНО (Half-Open): пробний запит");
                    return default;
                },
                OnClosed = args =>
                {
                    Log("Вимикач ЗАМКНЕНО (Closed): сервіс відновився");
                    return default;
                }
            })
            .Build();

        const int requests = 8;
        for (int i = 1; i <= requests; i++)
        {
            try
            {
                int free = await pipeline.ExecuteAsync(
                    async token => await seatMap.GetFreeSeatsAsync("Дюна", token));
                Log($"Запит {i}: вільних місць {free}");
            }
            catch (BrokenCircuitException)
            {
                Log($"Запит {i}: відхилено миттєво, сервіс не викликався");
            }
            catch (HttpRequestException ex)
            {
                Log($"Запит {i}: помилка — {ex.Message}");
            }

            await Task.Delay(400); 
        }

        Log($"Запитів: {requests}, реально дійшло до сервісу: {seatMap.RealCalls}");
        Console.WriteLine();
    }

    private static async Task TimeoutRetryFallbackScenario()
    {
        Console.WriteLine("3. Рекомендації: Timeout + Retry + Fallback");
        var service = new RecommendationService();
        var cachedMovies = new List<string> { "Популярне (з кешу): Барбі", "Паразити", "Титанік" };

        ResiliencePipeline<List<string>> pipeline = new ResiliencePipelineBuilder<List<string>>()
            .AddFallback(new FallbackStrategyOptions<List<string>>
            {
                ShouldHandle = new PredicateBuilder<List<string>>().Handle<TimeoutRejectedException>(),
                FallbackAction = args => Outcome.FromResultAsValueTask(cachedMovies),
                OnFallback = args =>
                {
                    Log("Fallback: показуємо збережений список популярних фільмів");
                    return default;
                }
            })
            .AddRetry(new RetryStrategyOptions<List<string>>
            {
                ShouldHandle = new PredicateBuilder<List<string>>().Handle<TimeoutRejectedException>(),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    Log($"Retry #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                OnTimeout = args =>
                {
                    Log($"Timeout: відповіді немає довше за {args.Timeout.TotalMilliseconds} мс");
                    return default;
                }
            })
            .Build();

        foreach (int responseTime in new[] { 2000, 100 }) 
        {
            Log($"Запит рекомендацій (сервіс відповідає за {responseTime} мс)...");
            var started = DateTime.Now;

            List<string> movies = await pipeline.ExecuteAsync(
                async token => await service.GetRecommendationsAsync(responseTime, token));

            Log($"Отримано за {(DateTime.Now - started).TotalMilliseconds:F0} мс: {string.Join(", ", movies)}");
        }
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
    }
}
