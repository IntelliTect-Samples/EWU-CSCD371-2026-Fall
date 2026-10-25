namespace Dag;

public class Dag<T>(IEqualityComparer<T>? comparer = null) where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependencies = new(comparer);

    public bool AddNode(T value) => _dependencies.TryAdd(value, new(_dependencies.Comparer));

    public bool AddDependency(T dependsOn, T dependent)
    {
        HashSet<T> dependencies = GetNode(dependent, nameof(dependent));
        GetNode(dependsOn, nameof(dependsOn));

        if (DependsOn(dependsOn, dependent))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        return dependencies.Add(dependsOn);
    }

    public IReadOnlyCollection<T> GetDependencies(T value) => [.. GetNode(value, nameof(value))];

    public IReadOnlyCollection<T> GetDependents(T value)
    {
        GetNode(value, nameof(value));

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

    public string ToMermaid()
    {
        List<string> lines = ["flowchart TD"];

        Dictionary<T, string> ids = new(_dependencies.Comparer);
        foreach (T node in _dependencies.Keys)
        {
            string id = $"n{ids.Count}";
            ids[node] = id;
            string label = $"{node}".Replace("\"", "#quot;");
            lines.Add($"    {id}[\"{label}\"]");
        }

        foreach ((T dependent, HashSet<T> dependencies) in _dependencies)
        {
            foreach (T dependsOn in dependencies)
            {
                lines.Add($"    {ids[dependsOn]} --> {ids[dependent]}");
            }
        }

        return string.Join(Environment.NewLine, lines);
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
    
    private HashSet<T> GetNode(T value, string paramName) => _dependencies.TryGetValue(value, out HashSet<T>? dependencies)
        ? dependencies
        : throw new ArgumentException("Node does not exist in the graph.", paramName);

}
