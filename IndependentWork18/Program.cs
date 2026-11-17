using System.Text;

namespace IndependentWork18;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Генерую 10 000 000 випадкових чисел...\n");
        List<int> numbers = Workload.GenerateNumbers(10_000_000);

        PerformanceTests.Run(numbers, new[] { 1_000_000, 5_000_000, 10_000_000 });

        SafetyDemo.Run(numbers.GetRange(0, 1_000_000));

        SafetyDemo.ShowOrder();
    }
}