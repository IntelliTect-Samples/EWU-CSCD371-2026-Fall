namespace Dag;

public sealed class Dag<T> where T : notnull
{
    // Nodes that directly depend on a given node
    private readonly Dictionary<T, HashSet<T>> _dependents = [];

    // Nodes that a given node directly depends on
    private readonly Dictionary<T, HashSet<T>> _dependencies = [];

    // Add a node if it does not already exist
    public void AddNode(T value)
    {
        if (_dependents.TryAdd(value, []))
        {
            _dependencies.Add(value, []);
        }
    }

    // Add an edge: dependsOn to dependent
    public void AddDependency(T dependsOn, T dependent)
    {
        // Check that both nodes exist
        if (!_dependents.TryGetValue(dependsOn, out HashSet<T>? dependents) ||
            !_dependencies.TryGetValue(dependent, out HashSet<T>? dependencies))
        {
            throw new ArgumentException(
                "Both nodes must already exist in the graph.");
        }

        // Do not add duplicate edges
        if (dependents.Contains(dependent))
        {
            return;
        }

        // Reject edges that would create a cycle
        if (EqualityComparer<T>.Default.Equals(dependsOn, dependent) ||
            HasPath(dependent, dependsOn))
        {
            throw new InvalidOperationException(
                "Adding this dependency would create a cycle.");
        }

        // Store the relationship in both directions
        dependents.Add(dependent);
        dependencies.Add(dependsOn);
    }

    // Return nodes this value directly depends on
    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        if (!_dependencies.TryGetValue(value, out HashSet<T>? nodes))
        {
            throw new ArgumentException(
                "The node does not exist in the graph.", nameof(value));
        }

        return [.. nodes];
    }

    // Return nodes that directly depend on this value
    public IReadOnlyCollection<T> GetDependents(T value)
    {
        if (!_dependents.TryGetValue(value, out HashSet<T>? nodes))
        {
            throw new ArgumentException(
                "The node does not exist in the graph.", nameof(value));
        }

        return [.. nodes];
    }

    // Check whether a path exists from start to target
    private bool HasPath(T start, T target)
    {
        HashSet<T> visited = [];
        Stack<T> stack = new();

        stack.Push(start);

        while (stack.Count > 0)
        {
            T current = stack.Pop();

            if (!visited.Add(current))
            {
                continue;
            }

            if (EqualityComparer<T>.Default.Equals(current, target))
            {
                return true;
            }

            foreach (T next in _dependents[current])
            {
                stack.Push(next);
            }
        }

        return false;
    }
}