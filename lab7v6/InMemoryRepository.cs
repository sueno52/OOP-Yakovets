namespace lab7v6;

public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new List<T>();

    public T GetById(int id)
    {
        foreach (T item in _items)
        {
            if (item.Id == id)
                return item;
        }
        throw new KeyNotFoundException($"Елемент з Id = {id} не знайдено.");
    }

    public IEnumerable<T> GetAll()
    {
        return _items.ToList();
    }

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (_items.Any(x => x.Id == item.Id))
            throw new ArgumentException($"Елемент з Id = {item.Id} вже існує.");

        _items.Add(item);
    }

    public void Update(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        int index = _items.FindIndex(x => x.Id == item.Id);
        if (index == -1)
            throw new KeyNotFoundException($"Елемент з Id = {item.Id} не знайдено.");

        _items[index] = item;
    }

    public void Delete(int id)
    {
        int index = _items.FindIndex(x => x.Id == id);
        if (index == -1)
            throw new KeyNotFoundException($"Елемент з Id = {id} не знайдено.");

        _items.RemoveAt(index);
    }
}
