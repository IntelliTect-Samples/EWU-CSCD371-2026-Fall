using System.Collections.Generic;
using System.Linq;

namespace GenericsHomework;
 

public class Circle<T> where T : class
{
	public string Name { get; }
	private readonly HashSet<T> _items = new();

	public Circle(string name) => Name = name;
	public bool Add(T item) => _items.Add(item);
	public bool Contains(T item) => _items.Contains(item);
	public IEnumerable<T> Items => _items;
    }

public class VennDiagram<T> where T: class
{
	private readonly Dictionary<string, Circle<T>> _circles = new();
	public IEnumerable<Circle<T>> Circles => _circles.Values;

	public Circle<T> AddCircle(string name)
	{
		if (_circles.TryGetValue(name, out var existing)) return existing;
		var c = new Circle<T>(name);
		_circles[name] = c;
		return c;
	}

	public Circle<T>? Get(string name) => _circles.TryGetValue(name, out var c) ? c : null;

	public IEnumerable<T> Intersection(params string[] names)
	{
		var sets = names.Select(Get).Where(c => c is not null).Select(c => c!.Items);
		if (!sets.Any()) return Enumerable.Empty<T>();
		return sets.Aggregate((a, b) => a.Intersect(b).ToList());
	}

	public IEnumerable<T> Union(params string[] names) => 
		names.Select(Get).Where(c => c is not null).SelectMany(c => c!.Items).Distinct().ToList();

	public IEnumerable<T> Difference(string a, string b)
	{
		var ca = Get(a);
		var cb = Get(b);
		if (ca is null) return Enumerable.Empty<T>();
		return cb is null ? ca.Items : ca.Items.Except(cb.Items).ToList();
	}
}