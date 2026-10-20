namespace Dag;

public class Dag<T> where T : notnull
{
    private readonly IEqualityComparer<T> _comparer;
    private readonly Dictionary<T, HashSet<T>> _dependencies;
    private readonly Dictionary<T, HashSet<T>> _dependents;

    public Dag(IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer ?? EqualityComparer<T>.Default;
        _dependencies = new Dictionary<T, HashSet<T>>(_comparer);
        _dependents = new Dictionary<T, HashSet<T>>(_comparer);
    }

    public bool AddNode(T value)
    {
        if (!_dependencies.TryAdd(value, new HashSet<T>(_comparer)))
        {
            return false;
        }

        _dependents.Add(value, new HashSet<T>(_comparer));
        return true;
    }
}
