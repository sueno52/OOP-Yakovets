using System.Diagnostics;

namespace InvertedIndexDemo;

public static class PerformanceTest
{
    private const int IndexRepeats = 10000;

    private static readonly string[] ExtraWords =
    {
        "cloud", "database", "server", "testing", "design", "pattern", "algorithm",
        "network", "security", "framework", "compiler", "memory", "thread", "query"
    };

    public static List<Document> GenerateDocuments(List<Document> baseDocs, int count)
    {
        var random = new Random(42);
        var result = new List<Document>();

        for (int i = 0; i < count; i++)
        {
            Document source = baseDocs[i % baseDocs.Count];
            string extra = string.Join(" ", Enumerable.Range(0, 5).Select(_ => ExtraWords[random.Next(ExtraWords.Length)]));

            result.Add(new Document
            {
                Id = i + 1,
                Title = source.Title,
                Content = source.Content + " " + extra
            });
        }
        return result;
    }

    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
        var sw = new Stopwatch();
        string normalizedWord = TextProcessor.Normalize(searchWord).First();

        int linearRepeats = Math.Max(3, 20000 / documents.Count);
        List<Document> linearResults = LinearSearch(documents, normalizedWord);
        sw.Start();
        for (int i = 0; i < linearRepeats; i++)
            linearResults = LinearSearch(documents, normalizedWord);
        sw.Stop();
        double linearMs = sw.Elapsed.TotalMilliseconds / linearRepeats;

        sw.Restart();
        var index = new InvertedIndex();
        foreach (var doc in documents)
            index.AddDocument(doc);
        sw.Stop();
        double buildMs = sw.Elapsed.TotalMilliseconds;

        List<Document> indexResults = index.SearchSingleWord(searchWord); 
        sw.Restart();
        for (int i = 0; i < IndexRepeats; i++)
            indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        double indexMs = sw.Elapsed.TotalMilliseconds / IndexRepeats;

        bool correct = linearResults.Count == indexResults.Count;
        Console.WriteLine($"| {documents.Count,6} | {linearMs,10:F4} | {indexMs,10:F4} | {buildMs,10:F2} | {linearMs / indexMs,8:F0}x | {linearResults.Count,5} / {indexResults.Count,-5} {(correct ? "OK" : "ПОМИЛКА")} |");
    }

    private static List<Document> LinearSearch(List<Document> documents, string normalizedWord)
    {
        return documents
            .Where(d => TextProcessor.Normalize(d.Title + " " + d.Content).Contains(normalizedWord))
            .ToList();
    }
}