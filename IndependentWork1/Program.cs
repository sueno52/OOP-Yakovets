using System.Text;
 
namespace IndependentWork1;
 
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Абонемент у спортзал");
        GymMembership gym = new GymMembership("Софія", 600, 3);
        gym.PrintInfo();
 
        gym.Extend(9); 
        gym.PrintInfo();
 
        gym.Months = -2;
        Console.WriteLine($"Місяців після спроби записати -2: {gym.Months}");
 
        Console.WriteLine();
        Console.WriteLine("Трекер води");
        WaterTracker water = new WaterTracker(2000);
        water.Drink(500);
        water.Drink(750);
        water.PrintStatus();
 
        water.Drink(800);
        water.PrintStatus();
 
        water.Drink(-100);

        Console.WriteLine();
        Console.WriteLine("Мобільний тариф");
        PhoneTariff tariff = new PhoneTariff("Смарт", 250, 20);
        tariff.PrintInfo();
 
        Console.WriteLine($"Вистачить 15 ГБ? {(tariff.HasEnoughInternet(15) ? "так" : "ні")}");
        Console.WriteLine($"Вистачить 30 ГБ? {(tariff.HasEnoughInternet(30) ? "так" : "ні")}");
 
        tariff.Name = "Смарт Плюс";
        tariff.PrintInfo();
    }
}