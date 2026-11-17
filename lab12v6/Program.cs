using System.Diagnostics;
using System.Text;

namespace lab12v6;

public class Program
{
    private const string LogPath = "app.log";
    private const string ServerUrl = "https://logs.example.com/api/upload";
    private static readonly Func<Exception, bool> IsTemporary =
        ex => ex is FileNotFoundException || ex is HttpRequestException;

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        TimeSpan delay = TimeSpan.FromMilliseconds(100);

        try
        {
            File.WriteAllLines(LogPath, new[]
            {
                "2026-11-17 10:00:01 INFO  Застосунок запущено",
                "2026-11-17 10:00:05 WARN  Повільна відповідь бази даних",
                "2026-11-17 10:00:09 ERROR Не вдалося зберегти замовлення #42"
            });

            string logContent = "";
            RunScenario("1. Читання логів (FileNotFoundException ×2)", () =>
            {
                var fileProcessor = new FileProcessor(failuresBeforeSuccess: 2);
                logContent = RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.GetLogContent(LogPath),
                    retryCount: 3, initialDelay: delay, shouldRetry: IsTemporary);

                Console.WriteLine("   Прочитано:");
                foreach (string line in logContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                    Console.WriteLine($"     {line}");
            });

            RunScenario("2. Відправка логів (HttpRequestException ×4)", () =>
            {
                var client = new NetworkClient(failuresBeforeSuccess: 4);
                RetryHelper.ExecuteWithRetry(
                    () => client.SendLogToServer(ServerUrl, logContent),
                    retryCount: 5, initialDelay: delay, shouldRetry: IsTemporary);
            });

            RunScenario("3. Спроби вичерпано (retryCount = 3)", () =>
            {
                var client = new NetworkClient(failuresBeforeSuccess: 4);
                RetryHelper.ExecuteWithRetry(
                    () => client.SendLogToServer(ServerUrl, logContent),
                    retryCount: 3, initialDelay: delay, shouldRetry: IsTemporary);
            });

            RunScenario("4. shouldRetry: порожній URL (ArgumentException)", () =>
            {
                var client = new NetworkClient(failuresBeforeSuccess: 0);
                RetryHelper.ExecuteWithRetry(
                    () => client.SendLogToServer("", logContent),
                    retryCount: 3, initialDelay: delay, shouldRetry: IsTemporary);
            });
        }
        finally
        {
            if (File.Exists(LogPath))
                File.Delete(LogPath);
            Console.WriteLine("Тимчасовий файл app.log видалено (finally).");
        }
    }
    private static void RunScenario(string title, Action scenario)
    {
        Console.WriteLine($"{title}");
        var sw = Stopwatch.StartNew();
        try
        {
            scenario();
            Console.WriteLine("   Результат: УСПІХ");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"   Результат: файл так і не знайдено — {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"   Результат: сервер так і не відповів — {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   Результат: помилка в даних — {ex.Message}");
        }
        finally
        {
            sw.Stop();
            Console.WriteLine($"   Час сценарію: {sw.ElapsedMilliseconds} мс\n");
        }
    }
}