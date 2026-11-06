using System.Diagnostics;
using System.Text;

namespace lab10v6;

public class Program
{
    private const int Repeats = 10_000;
    private static readonly List<string> Report = new List<string>();

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var movies = Data.Movies;
        var directors = Data.Directors;
        Console.WriteLine($"Дані: {directors.Count} режисерів, {movies.Count} фільмів\n");

        Console.WriteLine("1. Фільми Christopher Nolan після 2000 року");
        var q1 = Measure("1. Where + Join", "запитів", () => Queries.NolanAfter2000_Query(movies, directors));
        Measure("1. Where + Join", "методів", () => Queries.NolanAfter2000_Method(movies, directors));
        foreach (var m in q1)
            Console.WriteLine($"  {m.Title} ({m.ReleaseYear})");

        Console.WriteLine("\n2. Фільми та їхні режисери (Join)");
        var q2 = Measure("2. Join", "запитів", () => Queries.MoviesWithDirectors_Query(movies, directors));
        Measure("2. Join", "методів", () => Queries.MoviesWithDirectors_Method(movies, directors));
        foreach (var x in q2)
            Console.WriteLine($"  {x.Title} — {x.Director}");

        Console.WriteLine("\n3. Кількість фільмів кожного режисера (GroupJoin)");
        var q3 = Measure("3. GroupJoin + Count", "запитів", () => Queries.CountPerDirector_Query(movies, directors));
        Measure("3. GroupJoin + Count", "методів", () => Queries.CountPerDirector_Method(movies, directors));
        foreach (var x in q3)
            Console.WriteLine($"  {x.Director}: {x.Count}");

        Console.WriteLine("\n4. Найновіший фільм ");
        var q4 = Measure("4. OrderBy + First", "запитів", () => Queries.Newest_Query(movies));
        Measure("4. MaxBy", "методів", () => Queries.Newest_Method(movies));
        Console.WriteLine($"  {q4.Title} ({q4.ReleaseYear})");

        Console.WriteLine($"\nЧас виконання (перший запуск / середнє з {Repeats} повторів) ");
        Console.WriteLine("| Запит | Синтаксис | Перший запуск, мс | Середнє, мкс |");
        foreach (string row in Report)
            Console.WriteLine(row);
    }

    private static T Measure<T>(string name, string syntax, Func<T> query)
    {
        var sw = Stopwatch.StartNew();
        T result = query();
        sw.Stop();
        double firstMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < Repeats; i++)
            query();
        sw.Stop();
        double avgMicro = sw.Elapsed.TotalMilliseconds * 1000 / Repeats;

        Console.WriteLine($"  [синтаксис {syntax}] перший запуск {firstMs:F3} мс, середнє {avgMicro:F3} мкс");
        Report.Add($"| {name} | {syntax} | {firstMs:F3} | {avgMicro:F3} |");
        return result;
    }
}