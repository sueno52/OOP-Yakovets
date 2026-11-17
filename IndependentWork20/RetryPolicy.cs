namespace IndependentWork20;
public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = new Random();

    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        if (maxRetries < 0)
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторів не може бути від'ємною.");
        if (initialDelay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(initialDelay), "Затримка не може бути від'ємною.");
        if (jitterFactor < 0 || jitterFactor > 1)
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Jitter має бути від 0 до 1.");

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    public TimeSpan GetDelay(int retryAttempt)
    {
        double jitter = 1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor;
        return _initialDelay * Math.Pow(2, retryAttempt) * jitter;
    }

    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        shouldRetry ??= ex => true; 

        for (int retryAttempt = 0; ; retryAttempt++)
        {
            int attemptNumber = retryAttempt + 1;
            try
            {
                Log($"Спроба {attemptNumber}/{_maxRetries + 1}");
                T result = operation();
                Log($"Спроба {attemptNumber}: УСПІХ");
                return result;
            }
            catch (Exception ex)
            {
                Log($"Спроба {attemptNumber}: {ex.GetType().Name} — {ex.Message}");

                if (!shouldRetry(ex))
                {
                    Log("shouldRetry = false: помилка не тимчасова, повтори не допоможуть");
                    throw;
                }

                if (retryAttempt >= _maxRetries)
                {
                    Log($"Повтори вичерпано ({_maxRetries}), передаю виняток далі");
                    throw;
                }

                TimeSpan delay = GetDelay(retryAttempt);
                double baseMs = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt);
                Log($"Чекаю {delay.TotalMilliseconds:F0} мс (база {baseMs:F0} мс ± {_jitterFactor:P0})");
                Thread.Sleep(delay);
            }
        }
    }

    private static void Log(string message)
    {
        Console.WriteLine($"   [{DateTime.Now:HH:mm:ss.fff}] {message}");
    }
}
