namespace lab9v6;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public record RestockOrder(string ProductName, int Amount);

public record InventoryOperation(string Type, string ProductName, int Amount)
{
    public override string ToString() => $"{Type}: {ProductName} ({Amount} шт.)";
}

public class Inventory
{
    private readonly Dictionary<string, Product> _products = new Dictionary<string, Product>();

    private readonly HashSet<string> _categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private readonly Queue<RestockOrder> _restockQueue = new Queue<RestockOrder>();

    private readonly Stack<InventoryOperation> _history = new Stack<InventoryOperation>();


    public bool AddCategory(string category) => _categories.Add(category); 
    public bool HasCategory(string category) => _categories.Contains(category);
    public bool RemoveCategory(string category) => _categories.Remove(category);
    public IEnumerable<string> Categories => _categories.OrderBy(c => c);
    public int CategoryCount => _categories.Count;

    public void AddProduct(string name, string category, int amount)
    {
        if (_products.TryGetValue(name, out var product))
        {
            product.Quantity += amount;
        }
        else
        {
            _products[name] = new Product { Name = name, Category = category, Quantity = amount };
            _categories.Add(category);
        }

        _history.Push(new InventoryOperation("Додавання", name, amount));
    }

    public bool RemoveProduct(string name, int amount)
    {
        if (!_products.TryGetValue(name, out var product) || product.Quantity < amount)
        {
            Console.WriteLine($"  Помилка: не можна списати {amount} шт. товару \"{name}\"");
            return false;
        }

        product.Quantity -= amount;
        _history.Push(new InventoryOperation("Видалення", name, amount));
        return true;
    }

    public int HistoryCount => _history.Count;

    public InventoryOperation? PeekLastOperation()
    {
        return _history.Count > 0 ? _history.Peek() : null; 
    }

    public void Undo()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("  Немає операцій для скасування");
            return;
        }

        InventoryOperation last = _history.Pop();
        Product product = _products[last.ProductName];

        if (last.Type == "Додавання")
            product.Quantity -= last.Amount;
        else
            product.Quantity += last.Amount;

        Console.WriteLine($"  Скасовано -> {last}");
    }

    public int GetQuantity(string name) => _products.TryGetValue(name, out var p) ? p.Quantity : 0;


    public void EnqueueRestock(string productName, int amount)
    {
        _restockQueue.Enqueue(new RestockOrder(productName, amount));
    }

    public int RestockCount => _restockQueue.Count;

    public RestockOrder? PeekNextRestock()
    {
        return _restockQueue.Count > 0 ? _restockQueue.Peek() : null;
    }

    public void ProcessNextRestock()
    {
        RestockOrder order = _restockQueue.Dequeue();
        string category = _products.TryGetValue(order.ProductName, out var p) ? p.Category : "Без категорії";
        AddProduct(order.ProductName, category, order.Amount);
        Console.WriteLine($"  Оброблено: {order.ProductName} +{order.Amount} - тепер {GetQuantity(order.ProductName)} шт.");
    }
}