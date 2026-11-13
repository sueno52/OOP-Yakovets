namespace IndependentWork17;

public static class Pipelines
{
    public static List<CategoryTop> TopProductsByCategory(List<Sale> sales, List<Product> products)
    {
        var totals = from s in sales
                     group s by s.ProductId into g
                     select new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) };

        var result = from t in totals
                     join p in products on t.ProductId equals p.Id
                     group new { p.Name, t.Quantity } by p.Category into category
                     orderby category.Key
                     select new CategoryTop(
                         category.Key,
                         (from x in category
                          orderby x.Quantity descending, x.Name
                          select new ProductQuantity(x.Name, x.Quantity)).Take(3).ToList());

        return result.ToList();
    }

    public static List<CityRevenue> RevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        var result = from s in sales
                     join p in products on s.ProductId equals p.Id
                     join c in customers on s.CustomerId equals c.Id
                     group s.Quantity * p.UnitPrice by c.City into city  
                     let revenue = city.Sum()
                     orderby revenue descending
                     select new CityRevenue(city.Key, revenue, city.Count());

        return result.ToList();
    }

    public static List<MonthStats> SalesByMonth(List<Sale> sales, List<Product> products)
    {
        var result = from s in sales
                     join p in products on s.ProductId equals p.Id
                     group new { s.Quantity, Revenue = s.Quantity * p.UnitPrice }
                         by new { s.SaleDate.Year, s.SaleDate.Month } into month
                     orderby month.Key.Year, month.Key.Month
                     select new MonthStats(
                         month.Key.Year,
                         month.Key.Month,
                         month.Sum(x => x.Quantity),
                         month.Sum(x => x.Revenue));

        return result.ToList();
    }

    public static List<WeekdayStats> SalesByWeekday(List<Sale> sales, List<Product> products)
    {
        var result = from s in sales
                     join p in products on s.ProductId equals p.Id
                     let revenue = s.Quantity * p.UnitPrice
                     group new { p.Category, Revenue = revenue } by s.SaleDate.DayOfWeek into day
                     let total = day.Sum(x => x.Revenue)
                     orderby total descending
                     select new WeekdayStats(
                         day.Key,
                         day.Count(),
                         total,
                         Math.Round(total / day.Count(), 2),
                         (from x in day
                          group x.Revenue by x.Category into cat
                          orderby cat.Sum() descending
                          select cat.Key).First());

        return result.ToList();
    }
}