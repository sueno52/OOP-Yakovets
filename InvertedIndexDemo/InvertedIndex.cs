namespace InvertedIndexDemo;

public class InvertedIndex
{
    private Dictionary<string, HashSet<int>> _index;

    private Dictionary<int, Document> _documents;

    public InvertedIndex()
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
    }

    public int DocumentCount => _documents.Count;
    public int UniqueWords => _index.Keys.Count;

    public void AddDocument(Document doc)
    {
        _documents[doc.Id] = doc;

        foreach (string word in TextProcessor.Normalize(doc.Title + " " + doc.Content))
        {
            if (!_index.TryGetValue(word, out var ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }
            ids.Add(doc.Id); 
        }
    }

    public List<Document> SearchSingleWord(string word)
    {
        return ToDocuments(FindIds(word));
    }

    public List<Document> SearchMultipleWords(params string[] words)
    {
        HashSet<int>? result = null;

        foreach (string word in words)
        {
            var ids = FindIds(word);
            if (result == null)
                result = new HashSet<int>(ids); 
            else
                result.IntersectWith(ids);      

            if (result.Count == 0)
                break; 
        }

        return ToDocuments(result ?? new HashSet<int>());
    }

    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();
        foreach (string word in words)
        {
            result.UnionWith(FindIds(word));
        }
        return ToDocuments(result);
    }

    private HashSet<int> FindIds(string word)
    {
        string? normalized = TextProcessor.Normalize(word).FirstOrDefault();
        if (normalized != null && _index.TryGetValue(normalized, out var ids))
            return ids;
        return new HashSet<int>();
    }

    private List<Document> ToDocuments(HashSet<int> ids)
    {
        return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
    }
}