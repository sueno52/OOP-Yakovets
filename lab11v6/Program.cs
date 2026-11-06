using System.Text;

namespace lab11v6;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var playlists = Data.Playlists;

        Console.WriteLine("Дані");
        Console.WriteLine($"Плейлистів: {playlists.Count}, пісень: {Data.Songs.Count}, виконавців: {Data.Artists.Count}");
        foreach (var p in playlists)
            Console.WriteLine($"  {p.Name}: {p.Songs.Count} пісень");

        Console.WriteLine("\n1. Select vs SelectMany");
        IEnumerable<List<Song>> nested = playlists.Select(p => p.Songs);  
        IEnumerable<Song> flat = playlists.SelectMany(p => p.Songs);          
        Console.WriteLine($"Select:     {nested.Count()} елементи (кожен — окремий список пісень)");
        Console.WriteLine($"SelectMany: {flat.Count()} пісня в одному списку (з повторами)");

        Console.WriteLine("\n2. SelectMany: унікальні виконавці");
        var artists = playlists
            .SelectMany(p => p.Songs)        
            .Select(s => s.Artist)           
            .DistinctBy(a => a.Id)            
            .ToList();

        for (int i = 0; i < artists.Count; i++)
            Console.WriteLine($"  {i + 1}. {artists[i].Name}");

        var artistsQuery = (from p in playlists
                            from s in p.Songs
                            select s.Artist).DistinctBy(a => a.Id).ToList();
        Console.WriteLine($"Синтаксис запитів: {artistsQuery.Count} виконавців (той самий результат)");

        Console.WriteLine("\n3. Пагінація: усі пісні по 5 на сторінку");
        const int pageSize = 5;
        var allSongs = playlists
            .SelectMany(p => p.Songs)
            .DistinctBy(s => s.Id)
            .OrderBy(s => s.Id)
            .ToList();

        int totalPages = (int)Math.Ceiling(allSongs.Count / (double)pageSize);
        Console.WriteLine($"Усього пісень: {allSongs.Count}, сторінок: {totalPages}");

        for (int page = 1; page <= totalPages; page++)
        {
            Console.WriteLine($"Сторінка {page} з {totalPages}");
            var songsOnPage = allSongs.Skip((page - 1) * pageSize).Take(pageSize);
            foreach (var s in songsOnPage)
                Console.WriteLine($"  {s.Id}. {s.Title} — {s.Artist.Name} ({s.DurationText})");
        }
        var page2 = allSongs.Page(2, pageSize).Select(s => s.Title);
        Console.WriteLine($"Page(2, 5): {string.Join(", ", page2)}");
        Console.WriteLine($"Page(5, 5) (сторінки немає): {allSongs.Page(5, pageSize).Count()} пісень");

        Console.WriteLine("\n 4. Метод розширення TotalDuration()");
        foreach (var p in playlists)
            Console.WriteLine($"  {p.Name}: {p.Songs.TotalDuration().ToReadable()}");

        Console.WriteLine($"Усі пісні: {allSongs.TotalDuration().ToReadable()}");
        Console.WriteLine($"Сторінка 1: {allSongs.Page(1, pageSize).TotalDuration().ToReadable()}");

        var queenTime = allSongs.Where(s => s.Artist.Name == "Queen").TotalDuration();
        Console.WriteLine($"Пісні Queen: {queenTime.ToReadable()}");
    }
}