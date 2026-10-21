using System.Text;

namespace IndependentWork7;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Cache<string, string> cache = new Cache<string, string>(3);

        Console.WriteLine("Додаємо 3 елементи");
        cache.Add("cat", "кіт");
        cache.Add("dog", "собака");
        cache.Add("bird", "птах");
        Console.WriteLine($"Кількість: {cache.Count}");
        Console.WriteLine($"cat - {cache.Get("cat")}");
        Console.WriteLine($"dog - {cache.Get("dog")}");

        Console.WriteLine();
        Console.WriteLine("Додаємо 4-й елемент (кеш переповнено)");
        cache.Add("fish", "риба");
        Console.WriteLine($"Кількість: {cache.Count}");
        Console.WriteLine($"Чи є 'cat'? {cache.ContainsKey("cat")}  <- найстаріший, видалено");
        Console.WriteLine($"Чи є 'fish'? {cache.ContainsKey("fish")}");

        Console.WriteLine();
        Console.WriteLine("Оновлення існуючого ключа");
        cache.Add("dog", "пес");
        Console.WriteLine($"dog - {cache.Get("dog")}, кількість: {cache.Count}");

        Console.WriteLine();
        Console.WriteLine("Отримання неіснуючого ключа");
        try
        {
            cache.Get("cat");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }
}