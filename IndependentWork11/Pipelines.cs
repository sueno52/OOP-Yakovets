namespace IndependentWork11;

// Результати пайплайнів (record — коротко і з готовим ToString)
public record ProductQuantity(string Name, int Quantity);
public record CategoryTop(string Category, List<ProductQuantity> TopProducts);
public record CityRevenue(string City, decimal Revenue);
public record MonthStats(int Year, int Month, int Units, decimal Revenue);
public record BestCustomer(string Name, string City, int Purchases, decimal Total, string FavoriteCategory, decimal BiggestPurchase);

public static class Pipelines
{
    // Пайплайн 1: топ-3 продукти в кожній категорії
    public static List<CategoryTop> TopProductsByCategory(List<Sale> sales, List<Product> products)
    {
        return sales
            .GroupBy(s => s.ProductId)                                   // групуємо продажі за продуктом
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(s => s.Quantity) })
            .Join(products,                                              // додаємо назву і категорію
                  x => x.ProductId, p => p.Id,
                  (x, p) => new { p.Name, p.Category, x.Quantity })
            .GroupBy(x => x.Category)                                    // групуємо за категорією
            .Select(g => new CategoryTop(
                g.Key,
                g.OrderByDescending(x => x.Quantity)                     // 3 найпопулярніші
                 .Take(3)
                 .Select(x => new ProductQuantity(x.Name, x.Quantity))
                 .ToList()))
            .OrderBy(c => c.Category)
            .ToList();
    }

    // Пайплайн 2: загальний дохід за містами клієнтів
    public static List<CityRevenue> RevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers, x => x.CustomerId, c => c.Id,
                  (x, c) => new { c.City, x.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Sum(x => x.Revenue)))
            .OrderByDescending(c => c.Revenue)
            .ToList();
    }

    // Пайплайн 3: продажі за роками і місяцями
    public static List<MonthStats> SalesByMonth(List<Sale> sales, List<Product> products)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .Select(g => new MonthStats(g.Key.Year, g.Key.Month, g.Sum(x => x.Quantity), g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    public static List<BestCustomer> TopLoyalCustomers(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.CustomerId, p.Category, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Where(g => g.Count() >= 3)                                 
            .Join(customers, g => g.Key, c => c.Id, (g, c) => new BestCustomer(
                c.Name,
                c.City,
                g.Count(),
                g.Sum(x => x.Revenue),
                g.GroupBy(x => x.Category)                   
                 .OrderByDescending(cg => cg.Count())
                 .First().Key,
                g.Aggregate(0m, (max, x) => x.Revenue > max ? x.Revenue : max))) 
            .OrderByDescending(b => b.Total)
            .Take(5)
            .ToList();
    }
}