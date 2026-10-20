namespace Dag;

public class Dag<T>(IEqualityComparer<T>? comparer = null) where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependencies = new(comparer);

    public bool AddNode(T value) => _dependencies.TryAdd(value, new(_dependencies.Comparer));
}
