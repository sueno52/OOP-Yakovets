namespace IndependentWork6;
public abstract class FileProcessor
{
    private string _fileName;
    private string _content;
    private bool _isOpen;

    public string FileName
    {
        get { return _fileName; }
    }

    public bool IsOpen
    {
        get { return _isOpen; }
    }

    protected string Content
    {
        get { return _content; }
    }
    protected FileProcessor(string fileName, string content)
    {
        _fileName = fileName;
        _content = content;
        _isOpen = false;
    }

    public void Process()
    {
        Open();
        ProcessContent();
        Close();
    }

    private void Open()
    {
        _isOpen = true;
        Console.WriteLine($"Відкриття файлу {FileName}");
    }

    private void Close()
    {
        _isOpen = false;
        Console.WriteLine($"Закриття файлу {FileName}");
    }

    protected abstract void ProcessContent();
}

public class TextFileProcessor : FileProcessor
{
    public TextFileProcessor(string fileName, string content) : base(fileName, content)
    {
    }

    protected override void ProcessContent()
    {
        string[] words = Content.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"   Текст: {words.Length} слів");
    }
}
public class CsvFileProcessor : FileProcessor
{
    public CsvFileProcessor(string fileName, string content) : base(fileName, content)
    {
    }

    protected override void ProcessContent()
    {
        string[] lines = Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"   CSV: заголовок \"{lines[0]}\", записів: {lines.Length - 1}");
    }
}

public class XmlFileProcessor : FileProcessor
{
    public XmlFileProcessor(string fileName, string content) : base(fileName, content)
    {
    }

    protected override void ProcessContent()
    {
        int elements = Content.Split('<').Count(part => part.Length > 0 && part[0] != '/');
        Console.WriteLine($"   XML: {elements} елементи");
    }
}