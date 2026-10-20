using System.Collections;

namespace IndependentWork5;

public class MyCustomCollection : IEnumerable<string>
{
    private readonly List<string> _items = new List<string>();

    public void Add(string item)
    {
        _items.Add(item);
    }

    public int Count
    {
        get { return _items.Count; }
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (string item in _items)
        {
            yield return item; 
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}