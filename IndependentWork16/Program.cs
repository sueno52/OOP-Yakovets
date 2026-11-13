using System.Text;

namespace IndependentWork16;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var index = new InvertedIndex();
        List<Document> documents = LoadDocuments(index);

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");

        Console.WriteLine("\nSearchSingleWord(\"python\")");
        PrintList(index.SearchSingleWord("python"));

        Console.WriteLine("\nSearchMultipleWords(\"modern\", \"programming\") (AND) ");
        PrintList(index.SearchMultipleWords("modern", "programming"));

        Console.WriteLine("\nSearchPhrase(\"modern programming\") (точна фраза)");
        PrintList(index.SearchPhrase("modern programming"));

        Console.WriteLine("\nSearchAnyWord(\"java\", \"linq\") (OR)");
        PrintList(index.SearchAnyWord("java", "linq"));

        Console.WriteLine("\n SearchRanked(\"c#\", \"java\", \"python\") (OR + ранжування)");
        foreach (var (doc, score) in index.SearchRanked("c#", "java", "python"))
            Console.WriteLine($"{doc}  збігів: {score}");

        Console.WriteLine("\nPerformance: слово \"linq\" (середній час одного пошуку, мкс (ticks))");
        Console.WriteLine("| Документів | Лінійний, мкс (ticks) | Індекс, мкс (ticks) | Побудова індексу, мс | Швидше | Окупається після, пошуків | Знайдено L/I |");
        Console.WriteLine("| --- | --- | --- | --- | --- | --- | --- |");
        foreach (int count in new[] { 10, 100, 1000, 10000 })
        {
            var docs = PerformanceTest.GenerateDocuments(documents, count);
            PerformanceTest.CompareSearchMethods(docs, "linq");
        }

        Console.WriteLine("\nПідказка: слова через пробіл — AND-пошук, фраза в лапках — точна фраза.");
        while (true)
        {
            Console.Write("\nEnter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query == null || query.Trim().ToLower() == "exit")
                break;

            query = query.Trim();
            if (query.Length == 0)
                continue;

            List<Document> results;
            string[] words;

            if (query.Length > 1 && query.StartsWith('"') && query.EndsWith('"'))
            {
                string phrase = query.Trim('"');
                words = phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\nSearch Results (PHRASE)");
                results = index.SearchPhrase(phrase);
            }
            else
            {
                words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\nSearch Results (AND)");
                results = index.SearchMultipleWords(words);
            }

            foreach (var doc in results)
            {
                Console.WriteLine(doc);
                string preview = doc.Content.Substring(0, Math.Min(100, doc.Content.Length));
                Console.WriteLine($"  {TextProcessor.Highlight(preview, words)}...");
            }

            Console.WriteLine($"\nFound {results.Count} documents");
        }
    }

    private static void PrintList(List<Document> docs)
    {
        if (docs.Count == 0)
            Console.WriteLine("(нічого не знайдено)");
        foreach (var doc in docs)
            Console.WriteLine(doc);
    }

    private static List<Document> LoadDocuments(InvertedIndex index)
    {
        var documents = new List<Document>
        {
            new Document
            {
                Id = 1,
                Title = "Introduction to C#",
                Content = "C# is a modern object-oriented programming language developed by Microsoft."
            },
            new Document
            {
                Id = 2,
                Title = "Java Basics",
                Content = "Java is a popular programming language known for its portability and performance."
            },
            new Document
            {
                Id = 3,
                Title = "Python Overview",
                Content = "Python is a high-level programming language with simple syntax."
            },
            new Document
            {
                Id = 4,
                Title = "C# Advanced Features",
                Content = "C# supports LINQ, async/await, and many modern programming paradigms."
            },
            new Document
            {
                Id = 5,
                Title = "Comparing Languages",
                Content = "C#, Java, and Python are among the most popular programming languages today."
            }
        };

        foreach (var doc in documents)
            index.AddDocument(doc);

        return documents;
    }
}