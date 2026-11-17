namespace IndependentWork18;

public static class Workload
{
    public static List<int> GenerateNumbers(int count, int seed = 18)
    {
        var random = new Random(seed);
        var numbers = new List<int>(count);
        for (int i = 0; i < count; i++)
            numbers.Add(random.Next(1, 1_000_001));
        return numbers;
    }
    public static int CollatzSteps(int n)
    {
        long x = n; 
        int steps = 0;
        while (x != 1)
        {
            x = (x % 2 == 0) ? x / 2 : 3 * x + 1;
            steps++;
        }
        return steps;
    }
}