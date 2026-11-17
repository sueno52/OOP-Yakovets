namespace IndependentWork19;

public class BookingService
{
    private readonly HashSet<string> _takenSeats = new HashSet<string>();

    public string BookSeat(string session, int row, int seat)
    {
        if (row <= 0 || seat <= 0)
            throw new ArgumentOutOfRangeException(nameof(row), "Ряд і місце мають бути більшими за 0.");

        string key = $"{session}|{row}|{seat}";
        if (!_takenSeats.Add(key)) 
            throw new SeatAlreadyTakenException(session, row, seat);

        return $"Квиток: «{session}», ряд {row}, місце {seat}";
    }
}

public class PaymentGateway
{
    private int _failuresLeft;

    public PaymentGateway(int failuresBeforeSuccess) => _failuresLeft = failuresBeforeSuccess;

    public async Task<string> ChargeAsync(decimal amount, string idempotencyKey, CancellationToken token)
    {
        await Task.Delay(100, token);

        if (_failuresLeft > 0)
        {
            _failuresLeft--;
            throw new HttpRequestException("Платіжний шлюз тимчасово недоступний (503)");
        }

        return $"TX-{idempotencyKey}: списано {amount} грн";
    }
}

public class SeatMapService
{
    private int _failuresLeft;
    public int RealCalls { get; private set; } 

    public SeatMapService(int failuresBeforeRecovery) => _failuresLeft = failuresBeforeRecovery;

    public async Task<int> GetFreeSeatsAsync(string session, CancellationToken token)
    {
        RealCalls++;
        await Task.Delay(50, token);

        if (_failuresLeft > 0)
        {
            _failuresLeft--;
            throw new HttpRequestException("Сервіс схеми залу не відповідає");
        }

        return 42;
    }
}

public class RecommendationService
{
    public async Task<List<string>> GetRecommendationsAsync(int responseTimeMs, CancellationToken token)
    {
        await Task.Delay(responseTimeMs, token);
        return new List<string> { "Дюна: Частина друга", "Оппенгеймер", "Інтерстеллар" };
    }
}
