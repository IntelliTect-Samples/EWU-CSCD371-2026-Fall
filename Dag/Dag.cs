namespace Dag;
public sealed class Dag<T> where T : notnull
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

    public void AddNode(T value)
    {
        if (_dependencies.ContainsKey(value))
        {
            return;
        }

        _dependencies[value] = new HashSet<T>(_comparer);
        _dependents[value] = new HashSet<T>(_comparer);
    }

    public void AddDependency(T dependsOn, T dependent)
    {
        RequireNode(dependsOn);
        RequireNode(dependent);

        if (_dependents[dependsOn].Contains(dependent))
        {
            return;
        }

        if (IsReachable(dependent, dependsOn))
        {
            throw new InvalidOperationException("Adding this dependency creates a cycle.");
        }

        _dependents[dependsOn].Add(dependent);
        _dependencies[dependent].Add(dependsOn);
    }

    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        RequireNode(value);
        return _dependencies[value].ToArray();
    }

    public IReadOnlyCollection<T> GetDependents(T value)
    {
        RequireNode(value);
        return _dependents[value].ToArray();
    }

    public bool RemoveNode(T value)
    {
        if (!_dependencies.TryGetValue(value, out HashSet<T>? dependencies))
        {
            return false;
        }

        foreach (T dependsOn in dependencies)
        {
            _dependents[dependsOn].Remove(value);
        }

        foreach (T dependent in _dependents[value])
        {
            _dependencies[dependent].Remove(value);
        }

        _dependencies.Remove(value);
        _dependents.Remove(value);
        return true;
    }

    public string ToMermaid()
    {
        var ids = new Dictionary<T, string>(_comparer);
        var builder = new StringBuilder("flowchart TD\n");

        int index = 0;
        foreach (T node in _dependencies.Keys)
        {
            string id = "n" + index.ToString(CultureInfo.InvariantCulture);
            index++;
            ids[node] = id;

            string label = (node.ToString() ?? string.Empty).Replace("\"", "#quot;", StringComparison.Ordinal);
            builder.Append("    ").Append(id).Append("[\"").Append(label).Append("\"]\n");
        }

        foreach (T node in _dependencies.Keys)
        {
            foreach (T dependent in _dependents[node])
            {
                builder.Append("    ").Append(ids[node]).Append(" --> ").Append(ids[dependent]).Append('\n');
            }
        }

        return builder.ToString();
    }

    private void RequireNode(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (!_dependencies.ContainsKey(value))
        {
            throw new ArgumentException("The node does not exist in the graph.", paramName);
        }
    }

    private bool IsReachable(T start, T target)
    {
        var visited = new HashSet<T>(_comparer);
        var pending = new Stack<T>();
        pending.Push(start);

        while (pending.Count > 0)
        {
            T current = pending.Pop();
            if (_comparer.Equals(current, target))
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            foreach (T next in _dependents[current])
            {
                pending.Push(next);
            }
        }

        return false;
    }
}
