namespace lab6v6;

public class MediaItem
{
    private string _title = string.Empty;
    private int _duration;

    public string Title
    {
        get { return _title; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Назва не може бути порожньою.");
            _title = value;
        }
    }

    public int Duration
    {
        get { return _duration; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Тривалість має бути більшою за 0.");
            _duration = value;
        }
    }

    public MediaItem(string title, int duration)
    {
        Title = title;
        Duration = duration;
    }

    public virtual void Play()
    {
        Console.WriteLine($"Відтворення медіа \"{Title}\" ({Duration} хв).");
    }

    public string GetMediaType()
    {
        return "Медіа";
    }
}