namespace lab12v6;

public class FileProcessor
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    public FileProcessor(int failuresBeforeSuccess = 2)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public string GetLogContent(string path)
    {
        _calls++;
        if (_calls <= _failuresBeforeSuccess)
            throw new FileNotFoundException($"Файл '{path}' ще не створено (виклик {_calls})", path);

        return File.ReadAllText(path);
    }
}