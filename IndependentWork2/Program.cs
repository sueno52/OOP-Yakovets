using System.Globalization;
using System.Text;

namespace IndependentWork2;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        Console.WriteLine("Створення товарів");

        Product laptop = new Product(101, "Laptop", 35000m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {laptop}");

        Product mouse = new Product(102, "Mouse", 800m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {mouse}");

        Product laptopCopy = new Product(laptop);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {laptopCopy}");

        Console.WriteLine();
        Console.WriteLine($"Копія і оригінал — один і той самий об'єкт? {ReferenceEquals(laptop, laptopCopy)}");
    }
}