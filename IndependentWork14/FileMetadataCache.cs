namespace IndependentWork14;

public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{FileName}: {Size} байт, змінено {LastModified:dd.MM.yyyy}, hash {Hash}";
    }
}

public class FileMetadataCache
{
    private Dictionary<string, FileMetadata> _cache =
        new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

    public int Count
    {
        get { return _cache.Count; }
    }

    public bool Add(FileMetadata metadata)
    {
        bool isNew = !_cache.ContainsKey(metadata.FileName);
        _cache[metadata.FileName] = metadata;
        return isNew;
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

    public long TotalSize()
    {
        long total = 0;
        foreach (var metadata in _cache.Values)
            total += metadata.Size;
        return total;
    }
}