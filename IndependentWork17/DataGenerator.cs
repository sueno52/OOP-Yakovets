namespace IndependentWork17;

public static class DataGenerator
{
    private static readonly Dictionary<string, string[]> BooksByCategory = new()
    {
        ["Художня"] = new[] { "Кобзар", "Лісова пісня", "Тигролови", "Захар Беркут", "Intermezzo",
                              "Місто", "Сад Гетсиманський", "Жовтий князь", "Марія", "Солодка Даруся" },
        ["Дитяча"] = new[] { "Маленький принц", "Аліса в Країні Див", "Вінні-Пух", "Пеппі Довгапанчоха", "Мері Поппінс",
                             "Пітер Пен", "Мумі-тролі", "Хроніки Нарнії", "Чарлі і шоколадна фабрика", "Гаррі Поттер" },
        ["Навчальна"] = new[] { "C# для початківців", "Алгоритми", "Чистий код", "Патерни проєктування", "Бази даних",
                                "Комп'ютерні мережі", "Вища математика", "Фізика", "Історія України", "Англійська граматика" },
        ["Наукпоп"] = new[] { "Sapiens", "Коротка історія часу", "Егоїстичний ген", "Космос", "Ген",
                              "Мозок", "Фізика неможливого", "Походження видів", "Всесвіт у горіховій шкаралупі", "Чорні діри" },
        ["Саморозвиток"] = new[] { "Атомні звички", "Думай повільно", "Сила звички", "Емоційний інтелект", "7 звичок",
                                   "Людина в пошуках сенсу", "Потік", "Ікігай", "Стоїцизм", "Глибока робота" }
    };

    private static readonly string[] FirstNames = { "Олена", "Андрій", "Ірина", "Максим", "Софія", "Дмитро",
                                                     "Анна", "Богдан", "Марія", "Тарас", "Юлія", "Назар" };
    private static readonly string[] LastNames = { "Коваль", "Мельник", "Бойко", "Шевчук", "Ткач",
                                                   "Кравець", "Поліщук", "Савчук", "Лисенко", "Гончар" };
    private static readonly string[] Cities = { "Рівне", "Луцьк", "Житомир", "Тернопіль", "Вінниця", "Хмельницький" };

    public static List<Product> GenerateProducts(int seed = 17)
    {
        var random = new Random(seed);
        var products = new List<Product>();
        int id = 1;

        foreach (var pair in BooksByCategory)
        {
            foreach (string title in pair.Value)
            {
                products.Add(new Product
                {
                    Id = id++,
                    Name = title,
                    Category = pair.Key,
                    UnitPrice = random.Next(15, 91) * 10 
                });
            }
        }
        return products;
    }

    public static List<Customer> GenerateCustomers(int count, int seed = 17)
    {
        var random = new Random(seed);
        var customers = new List<Customer>();

        for (int i = 1; i <= count; i++)
        {
            customers.Add(new Customer
            {
                Id = i,
                Name = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}",
                City = Cities[random.Next(Cities.Length)]
            });
        }
        return customers;
    }

    public static List<Sale> GenerateSales(int count, int productCount, int customerCount, int seed = 17)
    {
        var random = new Random(seed);
        var start = new DateTime(2025, 1, 1);
        int days = (new DateTime(2026, 6, 30) - start).Days + 1;
        var sales = new List<Sale>();

        for (int i = 1; i <= count; i++)
        {
            sales.Add(new Sale
            {
                Id = i,
                ProductId = random.Next(1, productCount + 1),
                CustomerId = random.Next(1, customerCount + 1),
                Quantity = random.Next(1, 6),
                SaleDate = start.AddDays(random.Next(days))
            });
        }
        return sales;
    }
}