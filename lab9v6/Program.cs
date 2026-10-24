using System.Diagnostics;
using System.Text;

namespace lab9v6;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var inventory = new Inventory();

        Console.WriteLine("1. HashSet: унікальні категорії");
        inventory.AddProduct("Ноутбук", "Електроніка", 5);
        inventory.AddProduct("Миша", "Електроніка", 20);
        inventory.AddProduct("Футболка", "Одяг", 15);
        inventory.AddProduct("Чашка", "Дім", 30);
        Console.WriteLine("Додано 4 товари з 3 різних категорій");
        Console.WriteLine($"Категорії: {string.Join(", ", inventory.Categories)} (усього {inventory.CategoryCount})");

        Console.WriteLine($"Add(\"електроніка\") ще раз: {inventory.AddCategory("електроніка")}  - дублікат не додається");
        Console.WriteLine($"Contains(\"Одяг\"): {inventory.HasCategory("Одяг")}");
        Console.WriteLine($"Contains(\"Спорт\"): {inventory.HasCategory("Спорт")}");
        Console.WriteLine($"Add(\"Спорт\"): {inventory.AddCategory("Спорт")}, Remove(\"Спорт\"): {inventory.RemoveCategory("Спорт")}");
        Console.WriteLine($"Категорій зараз: {inventory.CategoryCount}");

        Console.WriteLine();
        Console.WriteLine("2. Queue: черга поповнення (FIFO)");
        inventory.EnqueueRestock("Миша", 10);
        inventory.EnqueueRestock("Футболка", 5);
        inventory.EnqueueRestock("Ноутбук", 2);
        Console.WriteLine($"У черзі {inventory.RestockCount} замовлення: Миша - Футболка - Ноутбук");
        Console.WriteLine($"Peek (перше в черзі): {inventory.PeekNextRestock()?.ProductName}");

        while (inventory.RestockCount > 0)
        {
            inventory.ProcessNextRestock(); 
        }
        Console.WriteLine($"Черга порожня: {inventory.RestockCount == 0}");

        Console.WriteLine();
        Console.WriteLine("3. Stack: історія операцій (LIFO)");
        inventory.RemoveProduct("Миша", 25);
        inventory.RemoveProduct("Чашка", 10);
        inventory.RemoveProduct("Ноутбук", 100); 
        Console.WriteLine($"Миша: {inventory.GetQuantity("Миша")} шт., Чашка: {inventory.GetQuantity("Чашка")} шт.");
        Console.WriteLine($"Операцій в історії: {inventory.HistoryCount}");
        Console.WriteLine($"Peek (остання операція): {inventory.PeekLastOperation()}");

        Console.WriteLine("Undo двічі:");
        inventory.Undo(); 
        inventory.Undo(); 
        Console.WriteLine($"Миша: {inventory.GetQuantity("Миша")} шт., Чашка: {inventory.GetQuantity("Чашка")} шт.");
        Console.WriteLine($"Операцій в історії: {inventory.HistoryCount}");

        Console.WriteLine();
        Console.WriteLine("4. Пошук: List vs HashSet (100 000 елементів, 10 000 пошуків)");
        var codes = Enumerable.Range(0, 100_000).Select(i => $"SKU-{i}").ToList();
        var codesSet = new HashSet<string>(codes);
        var random = new Random(42);
        var toFind = Enumerable.Range(0, 10_000).Select(_ => $"SKU-{random.Next(200_000)}").ToList();

        var sw = Stopwatch.StartNew();
        int foundInList = toFind.Count(c => codes.Contains(c));    
        sw.Stop();
        Console.WriteLine($"List.Contains:    {sw.Elapsed.TotalMilliseconds,8:F1} мс, знайдено {foundInList}");

        sw = Stopwatch.StartNew();
        int foundInSet = toFind.Count(c => codesSet.Contains(c)); 
        sw.Stop();
        Console.WriteLine($"HashSet.Contains: {sw.Elapsed.TotalMilliseconds,8:F1} мс, знайдено {foundInSet}");
    }
}