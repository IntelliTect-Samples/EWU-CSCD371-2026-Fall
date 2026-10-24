namespace Dag;

public class Dag<T>(IEqualityComparer<T>? comparer = null) where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependencies = new(comparer);

    public bool AddNode(T value) => _dependencies.TryAdd(value, new(_dependencies.Comparer));

    public bool AddDependency(T dependsOn, T dependent)
    {
        if (!_dependencies.TryGetValue(dependent, out HashSet<T>? dependencies))
        {
            throw new ArgumentException("Node does not exist in the graph.", nameof(dependent));
        }

        if (!_dependencies.ContainsKey(dependsOn))
        {
            throw new ArgumentException("Node does not exist in the graph.", nameof(dependsOn));
        }

        if (dependencies.Contains(dependsOn))
        {
            return false;
        }

        if (DependsOn(dependsOn, dependent))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        return dependencies.Add(dependsOn);
    }

    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        if (!_dependencies.TryGetValue(value, out HashSet<T>? dependencies))
        {
            throw new ArgumentException("Node does not exist in the graph.", nameof(value));
        }

        return [.. dependencies];
    }

    public IReadOnlyCollection<T> GetDependents(T value)
    {
        if (!_dependencies.ContainsKey(value))
        {
            throw new ArgumentException("Node does not exist in the graph.", nameof(value));
        }

        return [.. _dependencies.Where(node => node.Value.Contains(value)).Select(node => node.Key)];
    }

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

    private bool DependsOn(T start, T target)
    {
        HashSet<T> visited = new(_dependencies.Comparer);
        Stack<T> pending = new([start]);

        while (pending.TryPop(out T? current))
        {
            if (_dependencies.Comparer.Equals(current, target))
            {
                return true;
            }

            if (visited.Add(current))
            {
                foreach (T dependency in _dependencies[current])
                {
                    pending.Push(dependency);
                }
            }
        }

        return false;
    }
}
