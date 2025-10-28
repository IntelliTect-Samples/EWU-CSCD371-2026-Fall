namespace GenericsHomework;

/// <summary>
/// Represents a single circle in a Venn diagramm holding a collection of items of the same reference type
/// </summary>
public class Circle<T> where T : class
{
    //HashSet<T> automatically prevents duplicate items
    private readonly HashSet<T> _items = new();

    //Read-only that stores the name of the circle
    public string Name { get; }

    //Initializes a new Circle with a given name
    public Circle(string name) => Name = name;

    //Adds an item to the circle
    public void Add(T item) => _items.Add(item);

    //Checks whether the given items exists in the circle
    public bool Contains(T item) => _items.Contains(item);

    //Returns all items in the circle (read-only)
    public IEnumerable<T> GetItems() => _items;
    public override string ToString() => $"{Name}: {string.Join(", ", _items)}";

}


