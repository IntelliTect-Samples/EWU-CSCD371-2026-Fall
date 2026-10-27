using System.Globalization;
using System.Text;

namespace Dag;

public class Dag<T> where T : notnull
{
    private Dictionary<T, HashSet<T>> Dependencies { get; } = [];
    private Dictionary<T, HashSet<T>> Dependents { get; } = [];

    public bool AddNode(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!Dependencies.TryAdd(value, []))
        {
            return false;
        }

        Dependents.Add(value, []);
        return true;
    }

    public bool AddDependency(T dependsOn, T dependent)
    {
        ArgumentNullException.ThrowIfNull(dependsOn);
        ArgumentNullException.ThrowIfNull(dependent);

        if (!Dependents.TryGetValue(dependsOn, out HashSet<T>? dependents))
        {
            throw new ArgumentException("Add the node to the graph first.", nameof(dependsOn));
        }

        if (!Dependencies.TryGetValue(dependent, out HashSet<T>? dependencies))
        {
            throw new ArgumentException("Add the node to the graph first.", nameof(dependent));
        }

        if (dependents.Contains(dependent))
        {
            return false;
        }

        if (HasPath(dependent, dependsOn))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        dependents.Add(dependent);
        dependencies.Add(dependsOn);
        return true;
    }

    public T[] GetDependencies(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Dependencies.TryGetValue(value, out HashSet<T>? dependencies)
            ? [.. dependencies]
            : throw new ArgumentException("Add the node to the graph first.", nameof(value));
    }

    public T[] GetDependents(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Dependents.TryGetValue(value, out HashSet<T>? dependents)
            ? [.. dependents]
            : throw new ArgumentException("Add the node to the graph first.", nameof(value));
    }

    public bool RemoveNode(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!Dependencies.Remove(value, out HashSet<T>? dependencies))
        {
            return false;
        }

        foreach (T dependency in dependencies)
        {
            Dependents[dependency].Remove(value);
        }

        foreach (T dependent in Dependents[value])
        {
            Dependencies[dependent].Remove(value);
        }

        Dependents.Remove(value);
        return true;
    }

    public string ToMermaid()
    {
        StringBuilder builder = new();
        builder.AppendLine("flowchart TD");
        Dictionary<T, int> nodeIds = [];

        foreach (T value in Dependencies.Keys)
        {
            int nodeId = nodeIds.Count;
            nodeIds.Add(value, nodeId);
            string label = EscapeMermaidLabel(value.ToString() ?? string.Empty);
            builder.AppendLine(CultureInfo.InvariantCulture, $"    node{nodeId}[\"{label}\"]");
        }

        foreach ((T value, HashSet<T> dependents) in Dependents)
        {
            foreach (T dependent in dependents)
            {
                builder.AppendLine(CultureInfo.InvariantCulture, $"    node{nodeIds[value]} --> node{nodeIds[dependent]}");
            }
        }

        return builder.ToString();
    }

    private bool HasPath(T start, T target)
    {
        Stack<T> pending = new();
        pending.Push(start);
        HashSet<T> visited = [start];

        while (pending.TryPop(out T? current))
        {
            if (EqualityComparer<T>.Default.Equals(current, target))
            {
                return true;
            }

            foreach (T dependent in Dependents[current])
            {
                if (visited.Add(dependent))
                {
                    pending.Push(dependent);
                }
            }
        }

        return false;
    }

    private static string EscapeMermaidLabel(string label)
    {
        if (label.Length == 0)
        {
            return " ";
        }

        return label
            .Replace("#", "#35;", StringComparison.Ordinal)
            .Replace("&", "#38;", StringComparison.Ordinal)
            .Replace("\"", "#quot;", StringComparison.Ordinal)
            .Replace("<", "#60;", StringComparison.Ordinal)
            .Replace(">", "#62;", StringComparison.Ordinal)
            .Replace("`", "#96;", StringComparison.Ordinal)
            .ReplaceLineEndings("<br/>");
    }
}
