using System.Text;
using System.Globalization;

namespace Dag;

public sealed class Dag<T>(IEqualityComparer<T>? comparer = null) where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependents = [];
    //default equality(Equals/GetHashCode) comparer
    private readonly IEqualityComparer<T> _comparer = comparer ?? EqualityComparer<T>.Default;

    public void AddNode(T node)
    {
        if (!_dependents.ContainsKey(node))
        {
            _dependents[node] = [];
        }
    }
    public void AddDependency(T dependency, T dependent)
    {
        if (!_dependents.TryGetValue(dependency, out _))
        {
            throw new ArgumentException($"Node '{dependency}' does not exist.");
        }
        if (!_dependents.TryGetValue(dependent, out var dependencies))
        {
            throw new ArgumentException($"Node '{dependent}' does not exist.");
        }
        if (WouldCreateCycle(dependency, dependent))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }
        _dependents[dependency].Add(dependent);
    }
    public bool RemoveNode(T node)
    {
        // Removing the key drops all outgoing edges.
        if (!_dependents.Remove(node))
            return false;

        // Drop incoming edges from every remaining node.
        foreach (HashSet<T> targets in _dependents.Values)
            targets.Remove(node);

        return true;
    }

    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        EnsureNodeExists(value);
        return _dependents[value].ToList();
    }
    private void EnsureNodeExists(T value)
    {
        if (!_dependents.ContainsKey(value))
        {
            throw new ArgumentException($"Node {value} does not exist.");
        }
    }
    private bool WouldCreateCycle(T dependency, T dependent)
    {
        EnsureNodeExists(dependency);
        EnsureNodeExists(dependent);
        HashSet<T> visited = [];
        return CanReach(dependent, dependency, visited);
    }
    private bool CanReach(T current, T target, HashSet<T> visited)
    {

        if (EqualityComparer<T>.Default.Equals(current, target))
        {
            return true;
        }

        if (!visited.Add(current))
        {
            return false;
        }

        foreach (T neighbor in _dependents[current])
        {
            if (CanReach(neighbor, target, visited))
            {
                return true;
            }
        }

        return false;
    }
    public IReadOnlyCollection<T> GetDependents(T value)
    {
        EnsureNodeExists(value); //existence check added 
        return _dependents
            .Where(pair => pair.Value.Contains(value))
            .Select(pair => pair.Key)
            .ToList();
    }
    //Extra credit part 2:
    public string ToMermaid()
    {
        StringBuilder tower = new();
        tower.AppendLine("Mermaid flowchart");
        foreach (T node in _dependents.Keys)
        {
            string label = node!.ToString()!;
            string id = MakeMermaidID(label);

            tower.AppendLine(CultureInfo.InvariantCulture, $"    {id}");
        }
        foreach (KeyValuePair<T, HashSet<T>> pair in _dependents)
        {
            string source_id = MakeMermaidID(pair.Key!.ToString()!);
            foreach (T dependent in pair.Value)
            {
                string target_id = MakeMermaidID(dependent!.ToString()!);
                tower.AppendLine(CultureInfo.InvariantCulture, $"    {target_id} --> {source_id}");
            }
        }
        return tower.ToString();
    }
    private static string MakeMermaidID(String value)
    {
        StringBuilder builder = new();
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
            }
        }
        return builder.ToString();
    }
}