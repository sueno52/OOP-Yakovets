namespace IndependentWork16;

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

        foreach (string word in TextProcessor.Normalize(doc.Content))
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
            if (result == null)
                result = new HashSet<int>(FindIds(word)); 
            else
                result.IntersectWith(FindIds(word));

            if (result.Count == 0)
                break; 
        }

        return ToDocuments(result ?? new HashSet<int>());
    }

    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();
        foreach (string word in words)
            result.UnionWith(FindIds(word));
        return ToDocuments(result);
    }

    public List<Document> SearchPhrase(string phrase)
    {
        string[] phraseWords = TextProcessor.Tokenize(phrase).ToArray();
        if (phraseWords.Length == 0)
            return new List<Document>();

        var candidates = SearchMultipleWords(phraseWords);

        return candidates
            .Where(doc => ContainsSequence(TextProcessor.Tokenize(doc.Content).ToArray(), phraseWords))
            .ToList();
    }

    public List<(Document Doc, int Score)> SearchRanked(params string[] words)
    {
        var queryWords = new HashSet<string>(words.SelectMany(TextProcessor.Normalize));

        return SearchAnyWord(words)
            .Select(doc => (Doc: doc, Score: TextProcessor.Normalize(doc.Content).Count(w => queryWords.Contains(w))))
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Doc.Id)
            .ToList();
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
    private static bool ContainsSequence(string[] text, string[] phrase)
    {
        for (int i = 0; i + phrase.Length <= text.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < phrase.Length; j++)
            {
                if (text[i + j] != phrase[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
                return true;
        }
        return false;
    }
}