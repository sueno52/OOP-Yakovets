namespace IndependentWork8;

public class SearchIndex
{

    private Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

    public void AddDocument(int docId, string content)
    {
        string[] words = content.Split(content.Where(c => !char.IsLetterOrDigit(c)).Distinct().ToArray(),
                                       StringSplitOptions.RemoveEmptyEntries);

        foreach (string word in words)
        {
            if (!_invertedIndex.TryGetValue(word, out var docs))
            {
                docs = new HashSet<int>();
                _invertedIndex[word] = docs;
            }
            docs.Add(docId); 
        }
    }

    public HashSet<int> Search(string word)
    {
        if (_invertedIndex.TryGetValue(word, out var docs))
            return new HashSet<int>(docs); 
        return new HashSet<int>();
    }

    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
            return new HashSet<int>();

        var result = Search(words[0]);
        for (int i = 1; i < words.Length; i++)
            result.IntersectWith(Search(words[i]));
        return result;
    }
}