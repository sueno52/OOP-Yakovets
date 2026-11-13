using System.Text;

namespace IndependentWork14;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Кеш метаданих файлів (Dictionary)");
        var cache = new FileMetadataCache();
        cache.Add(new FileMetadata { FileName = "lab10.zip", Size = 15360, LastModified = new DateTime(2026, 11, 6), Hash = "9f2c" });
        cache.Add(new FileMetadata { FileName = "notes.docx", Size = 48200, LastModified = new DateTime(2026, 10, 20), Hash = "1ab7" });
        cache.Add(new FileMetadata { FileName = "cat.png", Size = 734003, LastModified = new DateTime(2026, 9, 1), Hash = "e44d" });

        Console.WriteLine($"Файлів у кеші: {cache.Count}");
        Console.WriteLine($"Get(\"NOTES.DOCX\") -> {cache.Get("NOTES.DOCX")}");
        Console.WriteLine($"Contains(\"music.mp3\"): {cache.Contains("music.mp3")}");

        bool isNew = cache.Add(new FileMetadata { FileName = "lab10.zip", Size = 16384, LastModified = new DateTime(2026, 11, 7), Hash = "7d01" });
        Console.WriteLine($"Повторний Add(\"lab10.zip\") новий? {isNew}  -> {cache.Get("lab10.zip")}");
        Console.WriteLine($"Загальний розмір: {cache.TotalSize()} байт");

        Console.WriteLine($"Remove(\"cat.png\"): {cache.Remove("cat.png")}, ще раз: {cache.Remove("cat.png")}");
        Console.WriteLine($"Get(\"cat.png\") -> {cache.Get("cat.png")?.ToString() ?? "не знайдено"}");
        Console.WriteLine($"Файлів у кеші: {cache.Count}");

        Console.WriteLine("\n2. Теги рецептів (HashSet)");
        var tags = new TagManager();
        tags.AddTag("borscht", "ukrainian");
        tags.AddTag("borscht", "soup");
        tags.AddTag("borscht", "dinner");
        tags.AddTag("varenyky", "ukrainian");
        tags.AddTag("varenyky", "dinner");
        tags.AddTag("varenyky", "dough");
        tags.AddTag("syrnyky", "ukrainian");
        tags.AddTag("syrnyky", "breakfast");
        tags.AddTag("syrnyky", "quick");

        Console.WriteLine($"AddTag(\"borscht\", \"Soup\") ще раз: {tags.AddTag("borscht", "Soup")}  - дублікат");
        Console.WriteLine($"borscht:  {Print(tags.GetTags("borscht"))}");
        Console.WriteLine($"varenyky: {Print(tags.GetTags("varenyky"))}");
        Console.WriteLine($"Усі унікальні (UnionWith): {Print(tags.GetAllUniqueTags())}");
        Console.WriteLine($"Спільні borscht і varenyky (IntersectWith): {Print(tags.FindCommonTags("borscht", "varenyky"))}");
        Console.WriteLine($"Тільки в borscht (ExceptWith): {Print(tags.FindTagsOnlyIn("borscht", "varenyky"))}");
        Console.WriteLine($"Рецепти з тегом ukrainian: {Print(tags.FindArticlesByTag("ukrainian"))}");

        tags.RemoveTag("syrnyky", "quick");
        Console.WriteLine($"syrnyky після RemoveTag(\"quick\"): {Print(tags.GetTags("syrnyky"))}");

        Console.WriteLine("\n3. Пошуковий індекс (Dictionary + HashSet)");
        var index = new SearchIndex();
        index.AddDocument(1, "Рівне — місто на Волині");
        index.AddDocument(2, "У Рівному є коледж інформаційних технологій");
        index.AddDocument(3, "Коледж готує програмістів, а Рівне пишається коледжем");

        Console.WriteLine($"Search(\"коледж\"): {Print(index.Search("коледж"))}");
        Console.WriteLine($"Search(\"рівне\"): {Print(index.Search("рівне"))}");
        Console.WriteLine($"SearchMultiple(\"рівне\", \"коледж\"): {Print(index.SearchMultiple("рівне", "коледж"))}");
        Console.WriteLine($"SearchAny(\"волині\", \"технологій\"): {Print(index.SearchAny("волині", "технологій"))}");
        Console.WriteLine($"Search(\"київ\"): {Print(index.Search("київ"))}");

        Console.WriteLine("\nCustom comparer");
        var students = new HashSet<Student>(new RecordBookComparer());
        Console.WriteLine($"Add(Яковець, залікова 2407): {students.Add(new Student { RecordBook = "2407", Name = "Яковець" })}");
        Console.WriteLine($"Add(Яковец, залікова 2407): {students.Add(new Student { RecordBook = "2407", Name = "Яковец" })}  - та сама залікова");
        Console.WriteLine($"Студентів у множині: {students.Count}");
    }

    private static string Print<T>(IEnumerable<T> items)
    {
        var list = items.OrderBy(x => x).ToList();
        return list.Count == 0 ? "(порожньо)" : string.Join(", ", list);
    }
}

public class Student
{
    public string RecordBook { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty;
}

public class RecordBookComparer : IEqualityComparer<Student>
{
    public bool Equals(Student? x, Student? y)
    {
        return x?.RecordBook == y?.RecordBook;
    }

    public int GetHashCode(Student obj)
    {
        return obj.RecordBook.GetHashCode();
    }
}