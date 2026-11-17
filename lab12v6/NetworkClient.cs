namespace lab12v6;
public class NetworkClient
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    public NetworkClient(int failuresBeforeSuccess = 4)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public void SendLogToServer(string url, string logContent)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL сервера не може бути порожнім.", nameof(url));

        _calls++;

        if (_calls <= _failuresBeforeSuccess)
            throw new HttpRequestException($"Сервер {url} недоступний (503), виклик {_calls}");

        Console.WriteLine($"   - Відправлено {logContent.Length} символів на {url}");
    }
}