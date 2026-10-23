using System.Text;

namespace InvertedIndexDemo;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var index = new InvertedIndex();
        List<Document> documents = LoadDocuments(index);

        Console.WriteLine($"Loaded {index.DocumentCount} documents");
        Console.WriteLine($"Index contains {index.UniqueWords} unique words");
        Console.WriteLine();

        Console.WriteLine("SearchAnyWord(\"java\", \"python\") (OR)");
        foreach (var doc in index.SearchAnyWord("java", "python"))
            Console.WriteLine(doc);
        Console.WriteLine();

        // ===== Вимірювання продуктивності =====
        Console.WriteLine("Performance: word \"microsoft\" (середній час одного пошуку, мс) ");
        Console.WriteLine("| Docs   | Linear, мс | Index, мс  | Build, мс  | Швидше  | Found (L / I) |");
        Console.WriteLine("| ------ | ---------- | ---------- | ---------- | ------- | ------------- |");
        foreach (int count in new[] { 10, 100, 1000, 10000 })
        {
            var docs = PerformanceTest.GenerateDocuments(documents, count);
            PerformanceTest.CompareSearchMethods(docs, "microsoft");
        }
        Console.WriteLine();

        while (true)
        {
            Console.Write("Enter search query (or 'exit'): ");
            string? query = Console.ReadLine();

            if (query == null || query.Trim().ToLower() == "exit")
                break;

            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                continue;

            Console.WriteLine("\nSearch Results (AND)");
            var results = index.SearchMultipleWords(words);
            foreach (var doc in results)
            {
                Console.WriteLine(doc);
                Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
            }

            Console.WriteLine($"\nFound {results.Count} documents\n");
        }
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