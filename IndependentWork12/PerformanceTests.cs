using System.Diagnostics;

namespace IndependentWork12;

public static class PerformanceTests
{
    public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n % 2 == 0) return n == 2;
        int limit = (int)Math.Sqrt(n);
        for (int i = 3; i <= limit; i += 2)
        {
            if (n % i == 0) return false;
        }
        return true;
    }
    public static List<int> GenerateNumbers(int count)
    {
        var random = new Random(42); 
        var numbers = new List<int>(count);
        for (int i = 0; i < count; i++)
            numbers.Add(random.Next(1, 10_000_000));
        return numbers;
    }

    public static void Run(int[] sizes)
    {
        Console.WriteLine($"Ядер процесора: {Environment.ProcessorCount}\n");
        Console.WriteLine("| Розмір | Операція | LINQ, мс | PLINQ, мс | Прискорення | Результат |");
        Console.WriteLine("| ------ | -------- | -------- | --------- | ----------- | --------- |");

        foreach (int size in sizes)
        {
            var numbers = GenerateNumbers(size);

            Compare(size, "легка (x*2)",
                () => numbers.Where(x => x % 2 == 0).Select(x => (long)x * 2).Sum(),
                () => numbers.AsParallel().Where(x => x % 2 == 0).Select(x => (long)x * 2).Sum());

            Compare(size, "важка (IsPrime)",
                () => numbers.Where(IsPrime).Select(x => (long)x).Sum(),
                () => numbers.AsParallel().Where(IsPrime).Select(x => (long)x).Sum());
        }
    }

    private static readonly HashSet<string> WarmedUp = new HashSet<string>();

    private static void Compare(int size, string operation, Func<long> linq, Func<long> plinq)
    {
        if (WarmedUp.Add(operation))
        {
            linq();
            plinq();
        }

        var sw = Stopwatch.StartNew();
        long linqResult = linq();
        sw.Stop();
        double linqMs = sw.Elapsed.TotalMilliseconds;

        sw = Stopwatch.StartNew();
        long plinqResult = plinq();
        sw.Stop();
        double plinqMs = sw.Elapsed.TotalMilliseconds;

        string same = linqResult == plinqResult ? "однаковий" : "РІЗНИЙ!";
        Console.WriteLine($"| {size / 1_000_000}M | {operation} | {linqMs:F1} | {plinqMs:F1} | {linqMs / plinqMs:F2}x | {same} |");
    }
}