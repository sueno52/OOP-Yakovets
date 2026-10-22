using System.Text;

namespace lab9_self_v6;

public class Movie
{
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public double Rating { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Числа");
        var numbers = new List<int> { 5, 12, 3, 20, 8 };

        var squares = CollectionUtils.Map(numbers, x => x * x);
        Console.WriteLine("Map (квадрати): " + string.Join(", ", squares));

        var even = CollectionUtils.Filter(numbers, x => x % 2 == 0);
        Console.WriteLine("Filter (парні): " + string.Join(", ", even));

        Console.WriteLine("MaxBy (найбільше): " + CollectionUtils.MaxBy(numbers, x => x));

        Console.Write("ForEach (x10): ");
        CollectionUtils.ForEach(numbers, x => Console.Write(x * 10 + " "));
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("Слова");
        var words = new List<string> { "кіт", "бегемот", "пес", "жирафа" };

        var lengths = CollectionUtils.Map(words, w => $"{w}={w.Length}");
        Console.WriteLine("Map (довжини): " + string.Join(", ", lengths));

        Console.WriteLine("MaxBy (найдовше): " + CollectionUtils.MaxBy(words, w => w.Length));

        var longWords = CollectionUtils.Filter(words, w => w.Length > 3);
        Console.WriteLine("Filter (> 3 літер): " + string.Join(", ", longWords));

        Console.Write("ForEach (великими): ");
        CollectionUtils.ForEach(words, w => Console.Write(w.ToUpper() + " "));
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("Фільми");
        var movies = new List<Movie>
        {
            new Movie { Title = "Тіні забутих предків", Year = 1965, Rating = 8.1 },
            new Movie { Title = "Поводир", Year = 2014, Rating = 7.6 },
            new Movie { Title = "Мавка. Лісова пісня", Year = 2023, Rating = 7.0 },
            new Movie { Title = "Довбуш", Year = 2023, Rating = 7.3 }
        };

        var titles = CollectionUtils.Map(movies, m => m.Title);
        Console.WriteLine("Map (назви): " + string.Join(", ", titles));

        var best = CollectionUtils.MaxBy(movies, m => m.Rating);
        Console.WriteLine($"MaxBy (найкраща оцінка): {best.Title} — {best.Rating}");

        Console.WriteLine("Filter (після 2015) + ForEach:");
        var newMovies = CollectionUtils.Filter(movies, m => m.Year > 2015);
        CollectionUtils.ForEach(newMovies, m => Console.WriteLine($"  {m.Title} ({m.Year})"));

        Console.WriteLine();
        Console.WriteLine("Порівняння з LINQ");
        Console.WriteLine("Map == Select: " + squares.SequenceEqual(numbers.Select(x => x * x)));
        Console.WriteLine("Filter == Where: " + even.SequenceEqual(numbers.Where(x => x % 2 == 0)));
        Console.WriteLine("MaxBy == LINQ MaxBy: " + (best == movies.MaxBy(m => m.Rating)));
    }
}