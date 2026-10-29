namespace Dag;

/// <summary>
/// A generic class representing a Directed Acyclic Graph (DAG).
/// </summary>
/// <typeparam name="T">The type of the graph's nodes.</typeparam>
public class Dag<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependencies = new();
    private readonly Dictionary<T, HashSet<T>> _dependents = new();

    /// <summary>
    /// Adds a node to the graph if it is not already present.
    /// </summary>
    /// <param name="value">The value of the node to add.</param>
    public void AddNode(T value)
    {
        if (_dependencies.ContainsKey(value))
        {
            return;
        }

        _dependencies.Add(value, []);
        _dependents.Add(value, []);
    }

    /// <summary>
    /// Adds a directed edge from <paramref name="dependsOn"/> to
    /// <paramref name="dependent"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Either node is missing, or the edge would create a cycle.
    /// </exception>
    public void AddDependency(T dependsOn, T dependent)
    {
        if (!_dependents.TryGetValue(dependsOn, out var dependentsOfPrerequisite)
            || !_dependencies.TryGetValue(dependent, out var dependenciesOfDependent))
        {
            throw new InvalidOperationException("Both nodes must exist in the graph.");
        }

        if (dependentsOfPrerequisite.Contains(dependent))
        {
            return;
        }

        if (WouldCreateCycle(dependsOn, dependent))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        dependentsOfPrerequisite.Add(dependent);
        dependenciesOfDependent.Add(dependsOn);
    }

    /// <summary>
    /// Returns the nodes that <paramref name="value"/> directly depends on.
    /// </summary>
    /// <exception cref="InvalidOperationException">The node is not in the graph.</exception>
    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        if (!_dependencies.TryGetValue(value, out var dependencies))
        {
            throw new InvalidOperationException("The node does not exist in the graph.");
        }

        return dependencies.ToArray();
    }

    /// <summary>
    /// Returns the nodes that directly depend on <paramref name="value"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The node is not in the graph.</exception>
    public IReadOnlyCollection<T> GetDependents(T value)
    {
        if (!_dependents.TryGetValue(value, out var dependents))
        {
            throw new InvalidOperationException("The node does not exist in the graph.");
        }

        return dependents.ToArray();
    }

    private bool WouldCreateCycle(T dependsOn, T dependent)
    {
        var visited = new HashSet<T>();
        var nodesToVisit = new Stack<T>();
        nodesToVisit.Push(dependent);

        while (nodesToVisit.TryPop(out var current))
        {
            if (EqualityComparer<T>.Default.Equals(current, dependsOn))
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            foreach (var next in _dependents[current])
            {
                nodesToVisit.Push(next);
            }
        }

        return false;
    }
}
