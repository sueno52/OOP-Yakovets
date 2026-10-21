namespace IndependentWork1;

public class PhoneTariff
{

    private string _name;
    private double _monthlyPrice;
    private int _internetGb;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public double MonthlyPrice
    {
        get { return _monthlyPrice; }
    }

    public PhoneTariff(string name, double monthlyPrice, int internetGb)
    {
        _name = name;
        _monthlyPrice = monthlyPrice;
        _internetGb = internetGb;
    }

    public double GetYearPrice()
    {
        return _monthlyPrice * 12;
    }

    public bool HasEnoughInternet(int neededGb)
    {
        return _internetGb >= neededGb;
    }
 
    public void PrintInfo()
    {
        Console.WriteLine($"Тариф \"{Name}\": {MonthlyPrice} грн/міс., {_internetGb} ГБ, за рік {GetYearPrice():F2} грн");
    }
}
