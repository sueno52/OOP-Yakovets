using System.Text;

namespace IndependentWork8;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Кеш метаданих (Dictionary)");
        var cache = new FileMetadataCache();
        cache.Add(new FileMetadata { FileName = "report.pdf", Size = 20480, LastModified = new DateTime(2026, 10, 1), Hash = "a1b2" });
        cache.Add(new FileMetadata { FileName = "photo.jpg", Size = 512000, LastModified = new DateTime(2026, 9, 15), Hash = "c3d4" });

        Console.WriteLine($"Файлів у кеші: {cache.Count}");
        Console.WriteLine($"Get(\"REPORT.PDF\"): {cache.Get("REPORT.PDF")}");
        Console.WriteLine($"Contains(\"music.mp3\"): {cache.Contains("music.mp3")}");
        cache.Remove("photo.jpg");
        Console.WriteLine($"Після Remove(\"photo.jpg\") файлів: {cache.Count}");

        Console.WriteLine();
        Console.WriteLine("2. Унікальні теги (HashSet)");
        var tags = new TagManager();
        tags.AddTag("post1", "csharp");
        tags.AddTag("post1", "oop");
        tags.AddTag("post1", "CSharp"); 
        tags.AddTag("post2", "csharp");
        tags.AddTag("post2", "dotnet");
        tags.AddTag("post2", "generics");

        Console.WriteLine($"Теги post1: {Print(tags.GetTags("post1"))}");
        Console.WriteLine($"Теги post2: {Print(tags.GetTags("post2"))}");
        Console.WriteLine($"Усі унікальні (UnionWith): {Print(tags.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні (IntersectWith): {Print(tags.FindCommonTags("post1", "post2"))}");
        Console.WriteLine($"Є в post2, немає в post1 (ExceptWith): {Print(tags.FindDifferentTags("post2", "post1"))}");

        Console.WriteLine();
        Console.WriteLine("3. Пошуковий індекс (Dictionary + HashSet)");
        var index = new SearchIndex();
        index.AddDocument(1, "C# це мова програмування");
        index.AddDocument(2, "Dictionary і HashSet — колекції C#");
        index.AddDocument(3, "Мова Python теж популярна мова");

        Console.WriteLine($"Search(\"мова\"): {Print(index.Search("мова"))}");
        Console.WriteLine($"Search(\"C\"): {Print(index.Search("C"))}");
        Console.WriteLine($"SearchMultiple(\"мова\", \"програмування\"): {Print(index.SearchMultiple("мова", "програмування"))}");
        Console.WriteLine($"Search(\"java\"): {Print(index.Search("java"))}");

        Console.WriteLine();
        Console.WriteLine("Custom comparer");
        var people = new HashSet<Person>(new PersonNameComparer());
        Console.WriteLine($"Додали John (25): {people.Add(new Person { Name = "John", Age = 25 })}");
        Console.WriteLine($"Додали John (30): {people.Add(new Person { Name = "John", Age = 30 })}  - те саме ім'я");
        Console.WriteLine($"Людей у множині: {people.Count}");
    }

    private static string Print<T>(IEnumerable<T> items)
    {
        var list = items.OrderBy(x => x).ToList();
        return list.Count == 0 ? "(порожньо)" : string.Join(", ", list);
    }
}

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonNameComparer : IEqualityComparer<Person>
{
    public bool Equals(Person? x, Person? y)
    {
        return x?.Name == y?.Name;
    }

    public int GetHashCode(Person obj)
    {
        return obj.Name.GetHashCode();
    }
}