namespace IndependentWork6;
public interface INotifier
{
    string ChannelName { get; }
    void Send(string message);
}

public interface ILogger
{
    void Log(string text);
}

public class EmailNotifier : INotifier, ILogger
{
    private string _email;

    public string ChannelName
    {
        get { return "Email"; }
    }

    public EmailNotifier(string email)
    {
        _email = email;
    }

    public void Send(string message)
    {
        Console.WriteLine($"[Email] Лист на {_email}: {message}");
        Log($"лист на {_email} відправлено");
    }

    public void Log(string text)
    {
        Console.WriteLine($"   (лог) {text}");
    }
}

public class SmsNotifier : INotifier
{
    private string _phone;

    public string ChannelName
    {
        get { return "SMS"; }
    }

    public SmsNotifier(string phone)
    {
        _phone = phone;
    }

    public void Send(string message)
    {
        string text = message.Length > 30 ? message.Substring(0, 30) + "..." : message;
        Console.WriteLine($"[SMS] На номер {_phone}: {text}");
    }
}

public class PushNotifier : INotifier
{
    private string _deviceName;

    public string ChannelName
    {
        get { return "Push"; }
    }

    public PushNotifier(string deviceName)
    {
        _deviceName = deviceName;
    }

    public void Send(string message)
    {
        Console.WriteLine($"[Push] На пристрій \"{_deviceName}\": {message}");
    }
}