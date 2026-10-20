namespace IndependentWork4;
public class Book : LibraryItem
{
    private string _author;

    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    public Book(string uniqueId, string title, int publicationYear, string author)
        : base(uniqueId, title, publicationYear)
    {
        _author = author;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"   Автор: {Author}");
    }
}