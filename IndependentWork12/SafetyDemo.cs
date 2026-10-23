using System.Collections.Concurrent;

namespace IndependentWork12;

public static class SafetyDemo
{
    public static void Run(List<int> numbers)
    {
        int expected = numbers.Count(x => x % 2 == 0);
        Console.WriteLine($"Очікувана кількість парних чисел: {expected}\n");

        Console.WriteLine("Проблема 1: counter++ у PLINQ (3 запуски)");
        for (int run = 1; run <= 3; run++)
        {
            int counter = 0;
            numbers.AsParallel().ForAll(x =>
            {
                if (x % 2 == 0)
                    counter++;
            });
            Console.WriteLine($"Запуск {run}: {counter} (втрачено {expected - counter})");
        }

        Console.WriteLine("\nПроблема 2: List.Add у PLINQ");
        for (int run = 1; run <= 3; run++)
        {
            var list = new List<int>();
            try
            {
                numbers.AsParallel().Where(x => x % 2 == 0).ForAll(x => list.Add(x));
                Console.WriteLine($"Запуск {run}: у списку {list.Count} (втрачено {expected - list.Count})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Запуск {run}: виняток {ex.GetBaseException().GetType().Name}");
            }
        }

        Console.WriteLine("\nВиправлення");

        int safeCounter = 0;
        numbers.AsParallel().ForAll(x =>
        {
            if (x % 2 == 0)
                Interlocked.Increment(ref safeCounter);
        });
        Console.WriteLine($"Interlocked.Increment: {safeCounter} {Check(safeCounter, expected)}");

        var lockedList = new List<int>();
        object locker = new object();
        numbers.AsParallel().Where(x => x % 2 == 0).ForAll(x =>
        {
            lock (locker)
            {
                lockedList.Add(x);
            }
        });
        Console.WriteLine($"lock + List:           {lockedList.Count} {Check(lockedList.Count, expected)}");

        var bag = new ConcurrentBag<int>();
        numbers.AsParallel().Where(x => x % 2 == 0).ForAll(x => bag.Add(x));
        Console.WriteLine($"ConcurrentBag:         {bag.Count} {Check(bag.Count, expected)}");

        int pure = numbers.AsParallel().Count(x => x % 2 == 0);
        Console.WriteLine($"AsParallel().Count():  {pure} {Check(pure, expected)}");
    }

    private static string Check(int actual, int expected)
    {
        return actual == expected ? "OK" : "ПОМИЛКА";
    }
}