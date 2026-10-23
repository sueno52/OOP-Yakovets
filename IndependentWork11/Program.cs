using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IndependentWork11;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        var products = DataGenerator.GenerateProducts();
        var customers = DataGenerator.GenerateCustomers(60);
        var sales = DataGenerator.GenerateSales(100, products.Count, customers.Count);
        Console.WriteLine($"Дані: {products.Count} товарів, {customers.Count} клієнтів, {sales.Count} продажів\n");

        var sw = Stopwatch.StartNew();
        var top = Pipelines.TopProductsByCategory(sales, products);
        sw.Stop();
        Console.WriteLine($"1. Найпопулярніші продукти за категоріями ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var category in top)
        {
            string items = string.Join(", ", category.TopProducts.Select(p => $"{p.Name} ({p.Quantity} шт.)"));
            Console.WriteLine($"{category.Category}: {items}");
        }

        sw = Stopwatch.StartNew();
        var cities = Pipelines.RevenueByCity(sales, products, customers);
        sw.Stop();
        Console.WriteLine($"\n2. Дохід за містами ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var city in cities)
            Console.WriteLine($"{city.City,-8} {city.Revenue,14:C}");

        sw = Stopwatch.StartNew();
        var months = Pipelines.SalesByMonth(sales, products);
        sw.Stop();
        Console.WriteLine($"\n3. Продажі за місяцями ({sw.Elapsed.TotalMilliseconds:F3} мс) ");
        foreach (var m in months)
            Console.WriteLine($"{m.Month:D2}.{m.Year}: {m.Units,3} шт., {m.Revenue,13:C}");

        sw = Stopwatch.StartNew();
        var loyal = Pipelines.TopLoyalCustomers(sales, products, customers);
        sw.Stop();
        Console.WriteLine($"\n4. Топ-5 постійних клієнтів ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var c in loyal)
            Console.WriteLine($"{c.Name} ({c.City}): {c.Purchases} покупки, {c.Total:C}, улюблене — {c.FavoriteCategory}, найбільша — {c.BiggestPurchase:C}");

        Console.WriteLine("\nЧас пайплайнів на 100 000 продажів (другий запуск, без JIT) ");
        var bigCustomers = DataGenerator.GenerateCustomers(5000);
        var bigSales = DataGenerator.GenerateSales(100_000, products.Count, bigCustomers.Count);
        Measure("1. Топ продуктів", () => Pipelines.TopProductsByCategory(bigSales, products));
        Measure("2. Дохід за містами", () => Pipelines.RevenueByCity(bigSales, products, bigCustomers));
        Measure("3. Продажі за місяцями", () => Pipelines.SalesByMonth(bigSales, products));
        Measure("4. Постійні клієнти", () => Pipelines.TopLoyalCustomers(bigSales, products, bigCustomers));
    }
    private static void Measure(string name, Action pipeline)
    {
        pipeline();
        var sw = Stopwatch.StartNew();
        pipeline();
        sw.Stop();
        Console.WriteLine($"{name,-24} {sw.Elapsed.TotalMilliseconds,8:F2} мс");
    }
}