namespace IndependentWork1;
public class GymMembership
{
    private string _ownerName;
    private double _pricePerMonth;
    private int _months;
 
    public string OwnerName
    {
        get { return _ownerName; }
    }

    public int Months
    {
        get { return _months; }
        set
        {
            if (value > 0)
                _months = value;
            else
                Console.WriteLine("Помилка: кількість місяців має бути більшою за 0");
        }
    }

    public GymMembership(string ownerName, double pricePerMonth, int months)
    {
        _ownerName = ownerName;
        _pricePerMonth = pricePerMonth;
        Months = months; 
    }
    public double GetTotalPrice()
    {
        double total = _pricePerMonth * _months;
 
        if (_months >= 12)
            total = total * 0.85;
        else if (_months >= 6)
            total = total * 0.95;
 
        return total;
    }

    public void Extend(int extraMonths)
    {
        Months = _months + extraMonths;
    }
 
    public void PrintInfo()
    {
        Console.WriteLine($"Абонемент: {OwnerName}, {Months} міс., до сплати {GetTotalPrice():F2} грн");
    }
}