using System.Collections.Generic;

namespace GenericsHomeWork;

public class Circle<T> where T : class
{
    private readonly HashSet<T> _items = new HashSet<T>();
    public string Name { get; }
    public Circle(string name)
    {
        Name = name;
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public IEnumerable<T> GetItems()
    {
        return _items;
    }
}