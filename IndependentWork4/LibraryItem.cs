namespace IndependentWork4;
public class LibraryItem
{
    private string _uniqueId;  
    private string _title;
    private int _publicationYear;

    public string UniqueId
    {
        get { return _uniqueId; }
    }

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int PublicationYear
    {
        get { return _publicationYear; }
        set { _publicationYear = value; }
    }

    public LibraryItem(string uniqueId, string title, int publicationYear)
    {
        _uniqueId = uniqueId;
        _title = title;
        _publicationYear = publicationYear;
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"[{UniqueId}] \"{Title}\", {PublicationYear} р.");
    }
}