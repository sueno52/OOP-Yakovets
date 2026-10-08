namespace lab6v6;

public class MovieMedia : MediaItem
{
    public string Director { get; set; }

    public MovieMedia(string title, int duration, string director) : base(title, duration)
    {
        Director = director;
    }

    public override void Play()
    {
        Console.WriteLine($"Перегляд фільму \"{Title}\" режисера {Director} ({Duration} хв).");
    }

    public void ShowTrailer()
    {
        Console.WriteLine($"Трейлер фільму \"{Title}\".");
    }
}