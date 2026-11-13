using System.Diagnostics;

namespace IndependentWork16;

public static class PerformanceTest
{
    private static readonly string[] Vocabulary =
    {
        "cloud", "database", "server", "testing", "design", "pattern", "algorithm",
        "network", "security", "framework", "compiler", "memory", "thread", "query"
    };

    public static List<Document> GenerateDocuments(List<Document> baseDocs, int count)
    {
        var random = new Random(7); 
        var result = new List<Document>();

        for (int i = 0; i < count; i++)
        {
            Document source = baseDocs[i % baseDocs.Count];
            var extra = new List<string>();
            for (int k = 0; k < 5; k++)
                extra.Add(Vocabulary[random.Next(Vocabulary.Length)]);

            result.Add(new Document
            {
                Id = i + 1,
                Title = source.Title,
                Content = source.Content + " " + string.Join(" ", extra)
            });
        }
        return result;
    }

    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
        var sw = new Stopwatch();
        string normalizedWord = TextProcessor.Normalize(searchWord).First();

        int linearRepeats = Math.Max(5, 10000 / documents.Count);
        var linearResults = LinearSearch(documents, normalizedWord);
        sw.Start();
        for (int i = 0; i < linearRepeats; i++)
            linearResults = LinearSearch(documents, normalizedWord);
        sw.Stop();
        double linearMicro = sw.Elapsed.TotalMilliseconds * 1000 / linearRepeats;
        long linearTicks = sw.ElapsedTicks / linearRepeats;

        sw.Restart();
        var index = new InvertedIndex();
        foreach (var doc in documents)
            index.AddDocument(doc);
        sw.Stop();
        double buildMs = sw.Elapsed.TotalMilliseconds;

        const int indexRepeats = 10000;
        var indexResults = index.SearchSingleWord(searchWord); 
        sw.Restart();
        for (int i = 0; i < indexRepeats; i++)
            indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        double indexMicro = sw.Elapsed.TotalMilliseconds * 1000 / indexRepeats;
        long indexTicks = sw.ElapsedTicks / indexRepeats;

        double saved = linearMicro - indexMicro;
        string payback = saved > 0 ? Math.Ceiling(buildMs * 1000 / saved).ToString() : "—";

        string check = linearResults.Count == indexResults.Count ? "OK" : "ПОМИЛКА";
        Console.WriteLine($"| {documents.Count} | {linearMicro:F2} ({linearTicks}) | {indexMicro:F2} ({indexTicks}) | {buildMs:F2} | {linearMicro / indexMicro:F0}x | {payback} | {linearResults.Count}/{indexResults.Count} {check} |");
    }

    private static List<Document> LinearSearch(List<Document> documents, string normalizedWord)
    {
        return documents
            .Where(d => TextProcessor.Normalize(d.Content).Contains(normalizedWord))
            .ToList();
    }
}