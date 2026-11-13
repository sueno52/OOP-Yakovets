namespace IndependentWork14;
public class TagManager
{
    private Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

    public bool AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out var tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _articleTags[articleId] = tags;
        }
        return tags.Add(tag);
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out var tags))
            tags.Remove(tag);
    }

    public HashSet<string> GetTags(string articleId)
    {
        if (_articleTags.TryGetValue(articleId, out var tags))
            return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase); // копія
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> GetAllUniqueTags()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in _articleTags.Values)
            result.UnionWith(tags);
        return result;
    }

    public HashSet<string> FindCommonTags(string articleId1, string articleId2)
    {
        var result = GetTags(articleId1);
        result.IntersectWith(GetTags(articleId2));
        return result;
    }

    public HashSet<string> FindTagsOnlyIn(string articleId, string otherArticleId)
    {
        var result = GetTags(articleId);
        result.ExceptWith(GetTags(otherArticleId));
        return result;
    }

    public HashSet<string> FindArticlesByTag(string tag)
    {
        var result = new HashSet<string>();
        foreach (var pair in _articleTags)
        {
            if (pair.Value.Contains(tag)) 
                result.Add(pair.Key);
        }
        return result;
    }
}