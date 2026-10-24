using System.Text;

namespace lab8v6;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Числа");
        var numbers = new List<int> { 3, 7, 1, 9, 5 };
        Console.WriteLine("Колекція: " + string.Join(", ", numbers));

        int sum = CollectionHelper<int>.Aggregate(numbers, 0, (acc, x) => acc + x);
        Console.WriteLine($"Сума: {sum}");

        int product = CollectionHelper<int>.Aggregate(numbers, 1, (acc, x) => acc * x);
        Console.WriteLine($"Добуток: {product}");

        int max = CollectionHelper<int>.Aggregate(numbers, int.MinValue, (acc, x) => x > acc ? x : acc);
        Console.WriteLine($"Найбільше: {max}");

        Console.WriteLine();
        Console.WriteLine("Рядки");
        var languages = new List<string> { "C#", "Java", "Python", "Go" };

        string joined = CollectionHelper<string>.Aggregate(languages, "", (acc, s) => acc == "" ? s : acc + ", " + s);
        Console.WriteLine($"Конкатенація: {joined}");

        int totalLength = CollectionHelper<string>.Aggregate(languages, 0, (acc, s) => acc + s.Length);
        Console.WriteLine($"Загальна кількість символів: {totalLength}");

        Console.WriteLine();
        Console.WriteLine("Товари");
        var products = new List<Product>
        {
            new Product { Name = "Ноутбук", Price = 25000m, Quantity = 2 },
            new Product { Name = "Миша", Price = 500m, Quantity = 5 },
            new Product { Name = "Клавіатура", Price = 1500m, Quantity = 3 }
        };

        decimal total = CollectionHelper<Product>.Aggregate(products, 0m, (acc, p) => acc + p.Price * p.Quantity);
        Console.WriteLine($"Загальна вартість складу: {total} грн");

        string names = CollectionHelper<Product>.Aggregate(products, "Товари:", (acc, p) => acc + " " + p.Name);
        Console.WriteLine(names);

        Console.WriteLine();
        Console.WriteLine("Порожня колекція");
        var empty = new List<int>();
        int emptySum = CollectionHelper<int>.Aggregate(empty, 0, (acc, x) => acc + x);
        Console.WriteLine($"Сума порожнього списку: {emptySum} (повертається seed)");
    }
}