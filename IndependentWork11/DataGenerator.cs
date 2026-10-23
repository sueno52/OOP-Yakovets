namespace IndependentWork11;

public static class DataGenerator
{
    private static readonly Dictionary<string, string[]> ProductsByCategory = new()
    {
        ["Електроніка"] = new[] { "Ноутбук", "Смартфон", "Планшет", "Навушники", "Монітор", "Миша", "Клавіатура", "Павербанк", "Колонка", "Вебкамера" },
        ["Книги"] = new[] { "Кобзар", "Тигролови", "Лісова пісня", "Захар Беркут", "Кайдашева сім'я", "Intermezzo", "Місто", "Сад Гетсиманський", "Чорна рада", "Маруся Чурай" },
        ["Одяг"] = new[] { "Футболка", "Худі", "Джинси", "Куртка", "Шарф", "Кросівки", "Шапка", "Сукня", "Светр", "Рукавиці" },
        ["Дім"] = new[] { "Чашка", "Плед", "Подушка", "Лампа", "Рушник", "Свічка", "Ваза", "Килимок", "Годинник", "Рамка для фото" },
        ["Спорт"] = new[] { "М'яч", "Гантелі", "Скакалка", "Килимок для йоги", "Пляшка", "Ракетка", "Велошолом", "Еспандер", "Рюкзак", "Термос" }
    };

    private static readonly string[] Names = { "Олена", "Андрій", "Ірина", "Максим", "Софія", "Дмитро", "Анна", "Богдан", "Марія", "Тарас" };
    private static readonly string[] Cities = { "Київ", "Львів", "Рівне", "Одеса", "Харків", "Дніпро" };

    public static List<Product> GenerateProducts(int seed = 42)
    {
        var random = new Random(seed);
        int id = 1;

        return ProductsByCategory
            .SelectMany(pair => pair.Value.Select(name => new Product
            {
                Id = id++,
                Name = name,
                Category = pair.Key,
                UnitPrice = random.Next(50, 5000)
            }))
            .ToList();
    }

    public static List<Customer> GenerateCustomers(int count, int seed = 42)
    {
        var random = new Random(seed);
        return Enumerable.Range(1, count)
            .Select(i => new Customer
            {
                Id = i,
                Name = $"{Names[random.Next(Names.Length)]} #{i}",
                City = Cities[random.Next(Cities.Length)]
            })
            .ToList();
    }

    public static List<Sale> GenerateSales(int count, int productCount, int customerCount, int seed = 42)
    {
        var random = new Random(seed);
        var start = new DateTime(2025, 1, 1);

        return Enumerable.Range(1, count)
            .Select(i => new Sale
            {
                Id = i,
                ProductId = random.Next(1, productCount + 1),
                Quantity = random.Next(1, 6),
                SaleDate = start.AddDays(random.Next(546)),
                CustomerId = random.Next(1, customerCount + 1)
            })
            .ToList();
    }
}