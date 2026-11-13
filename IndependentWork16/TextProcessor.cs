using System.Text.RegularExpressions;

namespace IndependentWork16;

public static class TextProcessor
{
    private const string WordPattern = @"[\w#+]+";

    private static readonly HashSet<string> StopWords = new HashSet<string>
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };
    public static IEnumerable<string> Tokenize(string text)
    {
        foreach (Match match in Regex.Matches(text.ToLowerInvariant(), WordPattern))
            yield return match.Value;
    }
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }
    public static IEnumerable<string> Normalize(string text)
    {
        return RemoveStopWords(Tokenize(text));
    }
    public static string Highlight(string text, IEnumerable<string> words)
    {
        var toHighlight = new HashSet<string>(words.SelectMany(Normalize));
        return Regex.Replace(text, WordPattern,
            m => toHighlight.Contains(m.Value.ToLowerInvariant()) ? $"[{m.Value}]" : m.Value);
    }
}