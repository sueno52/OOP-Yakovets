using System.Text;

namespace IndependentWork4;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Book book = new Book("BK-001", "Кобзар", 1840, "Тарас Шевченко");
        Journal journal = new Journal("JR-014", "Науковий вісник", 2024, 7);
        AudioBook audioBook = new AudioBook("AB-003", "Тигролови", 1944, "Іван Багряний", "Остап Ступка");

        Console.WriteLine("Окремі виклики DisplayDetails()");
        book.DisplayDetails();
        journal.DisplayDetails();
        audioBook.DisplayDetails();

        Console.WriteLine();
        Console.WriteLine("Поліморфізм: List<LibraryItem>");
        List<LibraryItem> library = new List<LibraryItem> { book, journal, audioBook };
        foreach (LibraryItem item in library)
        {
            item.DisplayDetails(); 
        }

    }
}