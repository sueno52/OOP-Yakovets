using System.Text;

namespace IndependentWork12;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Продуктивність LINQ vs PLINQ");
        PerformanceTests.Run(new[] { 1_000_000, 5_000_000, 10_000_000 });

        Console.WriteLine("\n 2. Побічні ефекти в PLINQ (1M елементів)");
        SafetyDemo.Run(PerformanceTests.GenerateNumbers(1_000_000));

    }
}