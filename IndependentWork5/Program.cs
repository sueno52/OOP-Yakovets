using System.Text;

namespace IndependentWork5;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        ObjectDemo();
        Console.WriteLine();

        EnumerableDemo();
        Console.WriteLine();

        StreamDemo();
    }
    private static void ObjectDemo()
    {
        Console.WriteLine("1. Поліморфізм через System.Object");

        List<object> items = new List<object>
        {
            new Song("Стефанія", "Kalush Orchestra", 172),
            new CinemaTicket("Довбуш", 5, 12, 180m),
            new Customer("Олена", "Коваль", "olena@example.com"),
            42,           
            "просто рядок" 
        };

        foreach (object item in items)
        {
            Console.WriteLine($"{item.GetType().Name,-14} - {item}");
        }
    }
    private static void EnumerableDemo()
    {
        Console.WriteLine("2. Поліморфізм через IEnumerable<string>");

        MyCustomCollection collection = new MyCustomCollection();
        collection.Add("C#");
        collection.Add("ООП");
        collection.Add("Поліморфізм");

        Console.WriteLine($"У MyCustomCollection {collection.Count} елементи:");
        foreach (string item in collection)
        {
            Console.WriteLine($" - {item}");
        }

        PrintAll(collection);
        PrintAll(new List<string> { "один", "два" });
        PrintAll(new string[] { "масив", "теж", "підходить" });
    }

    private static void PrintAll(IEnumerable<string> source)
    {
        Console.WriteLine($"{source.GetType().Name}: {string.Join(", ", source)}");
    }

    private static void StreamDemo()
    {
        Console.WriteLine("3. Поліморфізм через абстрактний клас Stream");

        string path = "test.txt";
        File.WriteAllText(path, "Hello from FileStream! This text is stored in a file.");

        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            ProcessStream(fileStream);
        }

        byte[] data = Encoding.UTF8.GetBytes("Hello from MemoryStream! This text is in memory.");
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            ProcessStream(memoryStream);
        }
    }

    private static void ProcessStream(Stream stream)
    {
        byte[] buffer = new byte[24];
        int bytesRead = stream.Read(buffer, 0, buffer.Length); 

        string text = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"{stream.GetType().Name}: довжина {stream.Length} байт, прочитано {bytesRead} байт - \"{text}\"");
    }
}