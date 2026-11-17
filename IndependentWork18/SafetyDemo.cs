using System.Diagnostics;

namespace IndependentWork18;

public static class SafetyDemo
{
    private const int BucketSize = 50; 
    private const int BucketCount = 12;  

    public static void Run(List<int> numbers)
    {
        Console.WriteLine($"2. Побічні ефекти в PLINQ ({numbers.Count:N0} чисел)");

        long expectedTotal = numbers.Select(x => (long)Workload.CollatzSteps(x)).Sum();
        Console.WriteLine($"Очікувана сума кроків: {expectedTotal:N0}");
        Console.WriteLine($"Очікувана кількість у гістограмі: {numbers.Count:N0}\n");

        Console.WriteLine("Без синхронізації (3 запуски)");
        for (int run = 1; run <= 3; run++)
        {
            long total = 0;
            int[] histogram = new int[BucketCount];

            numbers.AsParallel().ForAll(x =>
            {
                int steps = Workload.CollatzSteps(x);
                total += steps;                    
                histogram[Bucket(steps)]++;    
            });

            Console.WriteLine($"Запуск {run}: сума = {total:N0} ({Diff(total, expectedTotal)}), " +
                              $"у гістограмі = {histogram.Sum():N0} ({Diff(histogram.Sum(), numbers.Count)})");
        }

        Console.WriteLine("\n Виправлення");

        var sw = Stopwatch.StartNew();
        long lockedTotal = 0;
        int[] lockedHistogram = new int[BucketCount];
        object locker = new object();
        numbers.AsParallel().ForAll(x =>
        {
            int steps = Workload.CollatzSteps(x);
            lock (locker)
            {
                lockedTotal += steps;
                lockedHistogram[Bucket(steps)]++;
            }
        });
        sw.Stop();
        PrintFix("lock", lockedTotal, lockedHistogram.Sum(), expectedTotal, numbers.Count, sw);

        sw.Restart();
        long atomicTotal = 0;
        int[] atomicHistogram = new int[BucketCount];
        numbers.AsParallel().ForAll(x =>
        {
            int steps = Workload.CollatzSteps(x);
            Interlocked.Add(ref atomicTotal, steps);
            Interlocked.Increment(ref atomicHistogram[Bucket(steps)]);
        });
        sw.Stop();
        PrintFix("Interlocked", atomicTotal, atomicHistogram.Sum(), expectedTotal, numbers.Count, sw);

        sw.Restart();
        long pureTotal = numbers.AsParallel().Select(x => (long)Workload.CollatzSteps(x)).Sum();
        int pureHistogramCount = numbers.AsParallel()
            .GroupBy(x => Bucket(Workload.CollatzSteps(x)))
            .Sum(g => g.Count());
        sw.Stop();
        PrintFix("Sum + GroupBy (без побічних ефектів)", pureTotal, pureHistogramCount, expectedTotal, numbers.Count, sw);

        Console.WriteLine();
    }

    public static void ShowOrder()
    {
        Console.WriteLine("3. Порядок результатів");
        var source = Enumerable.Range(1, 12).ToList();

        var unordered = source.AsParallel().Select(x => { Workload.CollatzSteps(x * 1000); return x; }).ToList();
        var ordered = source.AsParallel().AsOrdered().Select(x => { Workload.CollatzSteps(x * 1000); return x; }).ToList();

        Console.WriteLine($"AsParallel():             {string.Join(", ", unordered)}");
        Console.WriteLine($"AsParallel().AsOrdered(): {string.Join(", ", ordered)}");
        Console.WriteLine();
    }

    private static int Bucket(int steps) => Math.Min(steps / BucketSize, BucketCount - 1);

    private static string Diff(long actual, long expected) =>
        actual == expected ? "збіглося" : $"втрачено {expected - actual:N0}";

    private static void PrintFix(string name, long total, int histogramCount, long expectedTotal, int expectedCount, Stopwatch sw)
    {
        bool ok = total == expectedTotal && histogramCount == expectedCount;
        Console.WriteLine($"{name}: сума = {total:N0}, у гістограмі = {histogramCount:N0} — " +
                          $"{(ok ? "ПРАВИЛЬНО" : "ПОМИЛКА")} ({sw.Elapsed.TotalMilliseconds:F1} мс)");
    }
}