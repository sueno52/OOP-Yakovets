using System.Text;

namespace lab6v6;

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        MediaItem media = new MediaItem("Подкаст про ООП", 45);
        BookMedia book = new BookMedia("Кобзар", 300, "Тарас Шевченко");
        MovieMedia movie = new MovieMedia("Тіні забутих предків", 97, "Сергій Параджанов");

        Console.WriteLine("Власні методи похідних класів");
        book.ReadSample();
        movie.ShowTrailer();

        Console.WriteLine();
        Console.WriteLine("Поліморфізм (virtual/override)");
        List<MediaItem> library = new List<MediaItem> { media, book, movie };
        foreach (MediaItem item in library)
        {
            item.Play(); 
        }

        Console.WriteLine();
        Console.WriteLine("override: посилання різних типів");
        MediaItem bookAsBase = book;
        bookAsBase.Play(); 
        book.Play();                     

        Console.WriteLine();
        Console.WriteLine("new: посилання різних типів");
        Console.WriteLine($"Через посилання MediaItem: {bookAsBase.GetMediaType()}");
        Console.WriteLine($"Через посилання BookMedia: {book.GetMediaType()}"); 
        Console.WriteLine($"Фільм (метод не прихований): {movie.GetMediaType()}");  
    }
}