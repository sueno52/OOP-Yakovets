using System.Diagnostics;
using System.Text;

namespace IndependentWork17;

public class Program
{
    private static readonly string[] MonthNames =
        { "", "січ", "лют", "бер", "кві", "тра", "чер", "лип", "сер", "вер", "жов", "лис", "гру" };

    private static readonly Dictionary<DayOfWeek, string> DayNames = new()
    {
        [DayOfWeek.Monday] = "Понеділок", [DayOfWeek.Tuesday] = "Вівторок", [DayOfWeek.Wednesday] = "Середа",
        [DayOfWeek.Thursday] = "Четвер", [DayOfWeek.Friday] = "П'ятниця", [DayOfWeek.Saturday] = "Субота",
        [DayOfWeek.Sunday] = "Неділя"
    };

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var extractTimer = Stopwatch.StartNew();
        var products = DataGenerator.GenerateProducts();
        var customers = DataGenerator.GenerateCustomers(100);
        var sales = DataGenerator.GenerateSales(500, products.Count, customers.Count);
        extractTimer.Stop();
        Console.WriteLine($"Дані: {products.Count} книг, {customers.Count} клієнтів, {sales.Count} продажів " +
                          $"(генерація {extractTimer.Elapsed.TotalMilliseconds:F2} мс)\n");

        ShowTopProducts(sales, products);
        ShowRevenueByCity(sales, products, customers);
        ShowSalesByMonth(sales, products);
        ShowSalesByWeekday(sales, products);

        Console.WriteLine("Продуктивність (перший запуск / середнє з 20 повторів, мс)");
        Console.WriteLine("| Пайплайн | 500 продажів: перший | 500: середнє | 50 000 продажів: перший | 50 000: середнє |");
        Console.WriteLine("| --- | --- | --- | --- | --- |");

        var bigSales = DataGenerator.GenerateSales(50_000, products.Count, customers.Count);
        PrintTimingRow("1. Топ-3 за категоріями",
            () => Pipelines.TopProductsByCategory(sales, products),
            () => Pipelines.TopProductsByCategory(bigSales, products));
        PrintTimingRow("2. Дохід за містами",
            () => Pipelines.RevenueByCity(sales, products, customers),
            () => Pipelines.RevenueByCity(bigSales, products, customers));
        PrintTimingRow("3. Продажі за місяцями",
            () => Pipelines.SalesByMonth(sales, products),
            () => Pipelines.SalesByMonth(bigSales, products));
        PrintTimingRow("4. Дні тижня",
            () => Pipelines.SalesByWeekday(sales, products),
            () => Pipelines.SalesByWeekday(bigSales, products));
    }

    private static void ShowTopProducts(List<Sale> sales, List<Product> products)
    {
        var sw = Stopwatch.StartNew(); 
        var result = Pipelines.TopProductsByCategory(sales, products);
        sw.Stop();

        Console.WriteLine($"1. Топ-3 книги в кожній категорії ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var category in result)
        {
            Console.WriteLine($"{category.Category}:");
            foreach (var p in category.TopProducts)
                Console.WriteLine($"   {p.Name} — {p.Quantity} шт.");
        }
        Console.WriteLine();
    }

    private static void ShowRevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        var sw = Stopwatch.StartNew();
        var result = Pipelines.RevenueByCity(sales, products, customers);
        sw.Stop();

        Console.WriteLine($"2. Дохід за містами ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var city in result)
            Console.WriteLine($"   {city.City,-13} {city.Revenue,10:N0} грн  ({city.SalesCount} продажів)");
        Console.WriteLine();
    }

    private static void ShowSalesByMonth(List<Sale> sales, List<Product> products)
    {
        var sw = Stopwatch.StartNew();
        var result = Pipelines.SalesByMonth(sales, products);
        sw.Stop();

        Console.WriteLine($"3. Продажі за місяцями ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var m in result)
            Console.WriteLine($"   {MonthNames[m.Month]} {m.Year}: {m.Units,4} шт., {m.Revenue,9:N0} грн");
        Console.WriteLine();
    }

    private static void ShowSalesByWeekday(List<Sale> sales, List<Product> products)
    {
        var sw = Stopwatch.StartNew();
        var result = Pipelines.SalesByWeekday(sales, products);
        sw.Stop();

        Console.WriteLine($"4. Продажі за днями тижня ({sw.Elapsed.TotalMilliseconds:F3} мс)");
        foreach (var d in result)
            Console.WriteLine($"   {DayNames[d.Day],-10} {d.Revenue,9:N0} грн, {d.SalesCount} продажів, " +
                              $"середній чек {d.AverageCheck:N0} грн, лідер: {d.BestCategory}");
        Console.WriteLine();
    }

    private static void PrintTimingRow<T>(string name, Func<T> small, Func<T> big)
    {
        var (smallFirst, smallAvg) = Measure(small);
        var (bigFirst, bigAvg) = Measure(big);
        Console.WriteLine($"| {name} | {smallFirst:F3} | {smallAvg:F3} | {bigFirst:F3} | {bigAvg:F3} |");
    }

    private static (double First, double Average) Measure<T>(Func<T> pipeline)
    {
        const int repeats = 20;

        var sw = Stopwatch.StartNew();
        pipeline();
        sw.Stop();
        double first = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < repeats; i++)
            pipeline();
        sw.Stop();

        return (first, sw.Elapsed.TotalMilliseconds / repeats);
    }
}