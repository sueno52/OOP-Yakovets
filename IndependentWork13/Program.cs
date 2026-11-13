using System.Text;

namespace IndependentWork13;
public record Book(string Title, string Author);

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var cache = new Cache<int, Book>(3);

        Console.WriteLine("FIFO: додаємо 3 книги");
        cache.Add(1, new Book("Кобзар", "Тарас Шевченко"));
        cache.Add(2, new Book("Тигролови", "Іван Багряний"));
        cache.Add(3, new Book("Лісова пісня", "Леся Українка"));
        Console.WriteLine($"Кількість: {cache.Count}");
        Console.WriteLine($"Книга 2: {cache.Get(2).Title}");

        Console.WriteLine("\nFIFO: додаємо 4-ту книгу (кеш повний)");
        cache.Add(4, new Book("Захар Беркут", "Іван Франко"));
        Console.WriteLine($"Кількість: {cache.Count}");
        Console.WriteLine($"Чи є книга 1? {cache.ContainsKey(1)}  - додана першою, видалена");
        Console.WriteLine($"Чи є книга 4? {cache.ContainsKey(4)}");

        Console.WriteLine("\n Отримання видаленої книги");
        try
        {
            cache.Get(1);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        var lru = new LruCache<int, Book>(3);

        Console.WriteLine("\nLRU: додаємо 3 книги і читаємо книгу 1");
        lru.Add(1, new Book("Кобзар", "Тарас Шевченко"));
        lru.Add(2, new Book("Тигролови", "Іван Багряний"));
        lru.Add(3, new Book("Лісова пісня", "Леся Українка"));
        Console.WriteLine($"Читаємо: {lru.Get(1).Title}");

        lru.Add(4, new Book("Захар Беркут", "Іван Франко"));
        Console.WriteLine($"Чи є книга 1? {lru.ContainsKey(1)}  - нею щойно користувались, залишилась");
        Console.WriteLine($"Чи є книга 2? {lru.ContainsKey(2)}  - найдовше не використовувалась, видалена");
    }
}