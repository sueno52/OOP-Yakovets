namespace IndependentWork8;

public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{FileName} ({Size} байт, змінено {LastModified:dd.MM.yyyy}, hash {Hash})";
    }
}

public class FileMetadataCache
{
 
    private Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

    public int Count
    {
        get { return _cache.Count; }
    }

    public void Add(FileMetadata metadata)
    {
        _cache[metadata.FileName] = metadata; 
    }

    public FileMetadata? Get(string fileName)
    {
        if (_cache.TryGetValue(fileName, out var metadata))
            return metadata;
        return null;
    }

    public bool Remove(string fileName)
    {
        return _cache.Remove(fileName);
    }

    public bool Contains(string fileName)
    {
        return _cache.ContainsKey(fileName);
    }
}