using System.Text;

namespace IndependentWork20;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var policy = new RetryPolicy(maxRetries: 4, initialDelay: TimeSpan.FromMilliseconds(200), jitterFactor: 0.25);
        Func<Exception, bool> isTransient = ex => ex is TimeoutException || ex is HttpRequestException;

        Console.WriteLine("0. Затримки з jitter (по 3 випадкові значення)");
        for (int attempt = 0; attempt < 4; attempt++)
        {
            double baseMs = 200 * Math.Pow(2, attempt);
            var samples = Enumerable.Range(0, 3).Select(_ => $"{policy.GetDelay(attempt).TotalMilliseconds:F0}");
            Console.WriteLine($"   Повтор {attempt + 1}: база {baseMs} мс, діапазон {baseMs * 0.75:F0}–{baseMs * 1.25:F0} мс, приклади: {string.Join(", ", samples)}");
        }

        Console.WriteLine("\n1. Синхронізація нотаток: 3 таймаути, потім успіх");
        int syncCalls = 0;
        string synced = policy.Execute(() =>
        {
            syncCalls++;
            if (syncCalls <= 3)
                throw new TimeoutException($"Хмара не відповіла за 5 с (виклик {syncCalls})");
            return "Синхронізовано 12 нотаток";
        }, isTransient);
        Console.WriteLine($"   Результат: {synced}\n");
        Console.WriteLine("2. Резервна копія: сервер постійно повертає 503");
        try
        {
            policy.Execute<bool>(() =>
                throw new HttpRequestException("Сервер резервних копій недоступний (503)"), isTransient);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"   Остаточна невдача: {ex.Message}\n");
        }

        Console.WriteLine("3. Вхід з простроченим токеном (UnauthorizedAccessException)");
        try
        {
            policy.Execute<string>(() =>
                throw new UnauthorizedAccessException("Токен доступу прострочено"), isTransient);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"   Без повторів: {ex.Message} — треба увійти заново");
        }
    }
}
