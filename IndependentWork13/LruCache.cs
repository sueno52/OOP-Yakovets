namespace IndependentWork13;
public class LruCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache;
    private readonly LinkedList<TKey> _keysOrder;

    public int Count
    {
        get { return _cache.Count; }
    }

    public LruCache(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Ємність кешу має бути більшою за 0.");

        _capacity = capacity;
        _cache = new Dictionary<TKey, TValue>();
        _keysOrder = new LinkedList<TKey>();
    }

    public void Add(TKey key, TValue value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            MoveToEnd(key);
            return;
        }

        if (_cache.Count == _capacity)
        {
            TKey leastUsedKey = _keysOrder.First!.Value;
            _keysOrder.RemoveFirst();
            _cache.Remove(leastUsedKey);
        }

        _cache.Add(key, value);
        _keysOrder.AddLast(key);
    }

    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            MoveToEnd(key); 
            return value;
        }

        throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
    }

    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }

    private void MoveToEnd(TKey key)
    {
        _keysOrder.Remove(key);
        _keysOrder.AddLast(key);
    }
}