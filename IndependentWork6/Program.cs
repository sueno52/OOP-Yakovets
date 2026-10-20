using System.Text;

namespace IndependentWork6;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Сценарій 1: система сповіщень (інтерфейс)");
        List<INotifier> notifiers = new List<INotifier>
        {
            new EmailNotifier("olena@example.com"),
            new SmsNotifier("+380501234567"),
            new PushNotifier("iPhone Олени")
        };

        foreach (INotifier notifier in notifiers)
        {
            notifier.Send("Ваше замовлення №125 вже в дорозі!");
        }

        Console.WriteLine();
        Console.WriteLine("Сценарій 2: обробка файлів (абстрактний клас) ");
        List<FileProcessor> processors = new List<FileProcessor>
        {
            new TextFileProcessor("notes.txt", "Інтерфейс описує що вміє клас а абстрактний клас описує ким він є"),
            new CsvFileProcessor("students.csv", "name,age\nОлена,19\nАндрій,20"),
            new XmlFileProcessor("library.xml", "<library><book>Кобзар</book><book>Тигролови</book></library>")
        };

        foreach (FileProcessor processor in processors)
        {
            processor.Process(); 
        }

    }
}