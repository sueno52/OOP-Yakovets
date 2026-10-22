namespace IndependentWork8;

public class TagManager
{

    private Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out var tags))
        {
            tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 
            _articleTags[articleId] = tags;
        }
        tags.Add(tag);
    }

    public void RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out var tags))
            tags.Remove(tag);
    }

    public HashSet<string> GetTags(string articleId)
    {
        if (_articleTags.TryGetValue(articleId, out var tags))
            return new HashSet<string>(tags, StringComparer.OrdinalIgnoreCase);
        return new HashSet<string>();
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

    public HashSet<string> FindDifferentTags(string articleId1, string articleId2)
    {
        var result = GetTags(articleId1);
        result.ExceptWith(GetTags(articleId2));
        return result;
    }
}