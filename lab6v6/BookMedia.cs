namespace lab6v6;

public class BookMedia : MediaItem
{
    public string Author { get; set; }

    public BookMedia(string title, int duration, string author) : base(title, duration)
    {
        Author = author;
    }

    public override void Play()
    {
        Console.WriteLine($"Читання книги \"{Title}\" автора {Author} ({Duration} хв).");
    }

    public void ReadSample()
    {
        Console.WriteLine($"Уривок з книги \"{Title}\": перші сторінки...");
    }


    public new string GetMediaType()
    {
        return "Книга";
    }
}