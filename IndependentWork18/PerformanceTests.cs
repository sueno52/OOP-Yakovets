using System.Diagnostics;

namespace IndependentWork18;

public static class PerformanceTests
{
    public static void Run(List<int> allNumbers, int[] sizes)
    {
        Console.WriteLine($"1. Продуктивність LINQ vs PLINQ (ядер процесора: {Environment.ProcessorCount})");
        Console.WriteLine("| Розмір | Операція | LINQ, мс | PLINQ, мс | Прискорення | Результати збігаються |");
        Console.WriteLine("| --- | --- | --- | --- | --- | --- |");

        var warmUp = allNumbers.GetRange(0, 10_000);
        HeavyLinq(warmUp); HeavyPlinq(warmUp); CheapLinq(warmUp); CheapPlinq(warmUp);

        foreach (int size in sizes)
        {
            List<int> numbers = allNumbers.GetRange(0, size);

            Compare(size, "Коллатц (важка)", () => HeavyLinq(numbers), () => HeavyPlinq(numbers));
            Compare(size, "x * 2 (легка)", () => CheapLinq(numbers), () => CheapPlinq(numbers));
        }
        Console.WriteLine();
    }

    private static long HeavyLinq(List<int> numbers) =>
        numbers
            .Where(x => x % 3 != 0)
            .Select(x => (long)Workload.CollatzSteps(x))
            .Sum();

    private static long HeavyPlinq(List<int> numbers) =>
        numbers
            .AsParallel()
            .Where(x => x % 3 != 0)
            .Select(x => (long)Workload.CollatzSteps(x))
            .Sum();

    private static long CheapLinq(List<int> numbers) =>
        numbers
            .Where(x => x % 2 == 0)
            .Select(x => (long)x * 2)
            .Sum();

    private static long CheapPlinq(List<int> numbers) =>
        numbers
            .AsParallel()
            .Where(x => x % 2 == 0)
            .Select(x => (long)x * 2)
            .Sum();

    private static void Compare(int size, string operation, Func<long> linq, Func<long> plinq)
    {
        var swLinq = Stopwatch.StartNew();
        long linqResult = linq();
        swLinq.Stop();

        var swPlinq = Stopwatch.StartNew();
        long plinqResult = plinq();
        swPlinq.Stop();

        double linqMs = swLinq.Elapsed.TotalMilliseconds;
        double plinqMs = swPlinq.Elapsed.TotalMilliseconds;
        string same = linqResult == plinqResult ? "так" : "НІ";

        Console.WriteLine($"| {size:N0} | {operation} | {linqMs:F1} | {plinqMs:F1} | x{linqMs / plinqMs:F2} | {same} |");
    }
}