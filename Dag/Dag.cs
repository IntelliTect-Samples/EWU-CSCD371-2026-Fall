namespace Dag;

public class Dag<T>(IEqualityComparer<T>? comparer = null) where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependencies = new(comparer);

    public bool AddNode(T value) => _dependencies.TryAdd(value, new(_dependencies.Comparer));

    public bool RemoveNode(T value)
    {
        if (!_dependencies.Remove(value))
        {
            return false;
        }

        foreach (HashSet<T> dependencies in _dependencies.Values)
        {
            dependencies.Remove(value);
        }

        return true;
    }
}
