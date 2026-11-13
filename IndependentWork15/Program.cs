using System.Text;

namespace IndependentWork15;
public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Температура (List<double>)");
        var temperatures = new List<double> { -3.5, 2.0, 7.0, -1.0, 4.5 };

        var fahrenheit = CollectionUtils.Map(temperatures, c => c * 9 / 5 + 32);
        Console.WriteLine("Map (у °F): " + string.Join("; ", CollectionUtils.Map(fahrenheit, f => f.ToString("F1"))));

        var frost = CollectionUtils.Filter(temperatures, t => t < 0);
        Console.WriteLine("Filter (мороз): " + string.Join("; ", frost));

        Console.WriteLine("MaxBy (найтепліше): " + CollectionUtils.MaxBy(temperatures, t => t));

        Console.Write("ForEach: ");
        CollectionUtils.ForEach(temperatures, t => Console.Write($"{t}°C  "));
        Console.WriteLine();

        Console.WriteLine("\n2. Студенти (List<string>)");
        var students = new List<string> { "Олена", "Максим", "Ірина", "Богдан", "Анастасія" };

        var initials = CollectionUtils.Map(students, s => s[0]);
        Console.WriteLine("Map (перші літери): " + string.Join(", ", initials));

        Console.WriteLine("MaxBy (найдовше ім'я): " + CollectionUtils.MaxBy(students, s => s.Length));

        var shortNames = CollectionUtils.Filter(students, s => s.Length <= 5);
        Console.WriteLine("Filter (до 5 літер): " + string.Join(", ", shortNames));

        Console.WriteLine("ForEach:");
        CollectionUtils.ForEach(students, s => Console.WriteLine($"  Привіт, {s}!"));

        Console.WriteLine("\n3. Товари (List<Product>)");
        var products = new List<Product>
        {
            new Product { Name = "Ноутбук", Price = 25000 },
            new Product { Name = "Мишка", Price = 500 },
            new Product { Name = "Клавіатура", Price = 1500 },
            new Product { Name = "Монітор", Price = 7200 },
            new Product { Name = "Навушники", Price = 2300 }
        };

        var names = CollectionUtils.Map(products, p => p.Name);
        Console.WriteLine("Map (назви): " + string.Join(", ", names));

        var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine($"MaxBy (найдорожчий): {mostExpensive.Name} — {mostExpensive.Price} грн");

        Console.WriteLine("Filter (від 1000 до 10000 грн) + ForEach:");
        var middle = CollectionUtils.Filter(products, p => p.Price >= 1000 && p.Price <= 10000);
        CollectionUtils.ForEach(middle, p => Console.WriteLine($"  {p.Name}: {p.Price} грн"));

        Predicate<Product> isCheap = p => p.Price < 2000;
        Action<Product> print = p => Console.WriteLine($"  {p.Name}: {p.Price} грн");
        Console.WriteLine("Дешевші за 2000 грн (делегати у змінних):");
        CollectionUtils.ForEach(CollectionUtils.Filter(products, isCheap), print);

        Console.WriteLine("\nПорівняння з LINQ");
        Console.WriteLine("Map == Select: " + names.SequenceEqual(products.Select(p => p.Name)));
        Console.WriteLine("Filter == Where: " + frost.SequenceEqual(temperatures.Where(t => t < 0)));
        Console.WriteLine("MaxBy == LINQ MaxBy: " + (mostExpensive == products.MaxBy(p => p.Price)));
    }
}