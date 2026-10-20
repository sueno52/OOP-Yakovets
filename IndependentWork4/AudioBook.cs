namespace IndependentWork4;
public class AudioBook : Book
{
    private string _narrator;

    public string Narrator
    {
        get { return _narrator; }
        set { _narrator = value; }
    }

    public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
        : base(uniqueId, title, publicationYear, author)
    {
        _narrator = narrator;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails(); 
        Console.WriteLine($"   Оповідач: {Narrator}");
    }
}