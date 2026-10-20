namespace IndependentWork1;
public class WaterTracker
{
    private int _dailyGoal;
    private int _drunk;
    public int DailyGoal
    {
        get { return _dailyGoal; }
        set
        {
            if (value > 0)
                _dailyGoal = value;
            else
                Console.WriteLine("Помилка: ціль має бути більшою за 0");
        }
    }
    public int Drunk
    {
        get { return _drunk; }
    }

    public WaterTracker(int dailyGoal)
    {
        DailyGoal = dailyGoal;
        _drunk = 0;
    }

    public void Drink(int ml)
    {
        if (ml <= 0)
        {
            Console.WriteLine("Помилка: кількість води має бути більшою за 0");
            return;
        }
        _drunk += ml;
    }

    public double GetProgress()
    {
        return (double)_drunk / _dailyGoal * 100;
    }

    public bool IsGoalReached()
    {
        return _drunk >= _dailyGoal;
    }
 
    public void PrintStatus()
    {
        string result = IsGoalReached() ? "ціль виконано!" : "ще треба пити";
        Console.WriteLine($"Вода: {Drunk}/{DailyGoal} мл ({GetProgress():F0}%) — {result}");
    }
}