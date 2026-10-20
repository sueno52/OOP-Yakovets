namespace IndependentWork1;
 
// Тариф мобільного оператора
public class PhoneTariff
{
    // приватні поля
    private string _name;
    private double _monthlyPrice;
    private int _internetGb;
 
    // назву тарифу можна змінити
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
 
    // ціна тільки для читання
    public double MonthlyPrice
    {
        get { return _monthlyPrice; }
    }
 
    // конструктор
    public PhoneTariff(string name, double monthlyPrice, int internetGb)
    {
        _name = name;
        _monthlyPrice = monthlyPrice;
        _internetGb = internetGb;
    }
 
    // скільки коштує тариф за рік
    public double GetYearPrice()
    {
        return _monthlyPrice * 12;
    }
 
    // чи вистачить інтернету на місяць
    public bool HasEnoughInternet(int neededGb)
    {
        return _internetGb >= neededGb;
    }
 
    public void PrintInfo()
    {
        Console.WriteLine($"Тариф \"{Name}\": {MonthlyPrice} грн/міс., {_internetGb} ГБ, за рік {GetYearPrice():F2} грн");
    }
}