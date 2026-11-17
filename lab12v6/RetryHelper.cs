namespace lab12v6;
public static class RetryHelper
{
    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool>? shouldRetry = null)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Кількість повторів не може бути від'ємною.");

        if (initialDelay == default)
            initialDelay = TimeSpan.FromMilliseconds(100);

        shouldRetry ??= ex => true;

        int maxAttempts = retryCount + 1; 

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                Log($"Спроба {attempt}/{maxAttempts}...");
                T result = operation();
                Log($"Спроба {attempt}: успіх");
                return result;
            }
            catch (Exception ex)
            {
                Log($"Спроба {attempt}: {ex.GetType().Name} — {ex.Message}");

                if (!shouldRetry(ex))
                {
                    Log("Цей виняток не тимчасовий — повторювати не будемо.");
                    throw;
                }

                if (attempt == maxAttempts)
                {
                    Log($"Спроби вичерпано ({maxAttempts}).");
                    throw;
                }

                TimeSpan delay = initialDelay * Math.Pow(2, attempt - 1);
                Log($"Чекаю {delay.TotalMilliseconds} мс і пробую знову");
                Thread.Sleep(delay);
            }
        }
    }
    public static void ExecuteWithRetry(
        Action operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool>? shouldRetry = null)
    {
        ExecuteWithRetry(() =>
        {
            operation();
            return true;
        }, retryCount, initialDelay, shouldRetry);
    }

    private static void Log(string message)
    {
        Console.WriteLine($"   [{DateTime.Now:HH:mm:ss.fff}] {message}");
    }
}