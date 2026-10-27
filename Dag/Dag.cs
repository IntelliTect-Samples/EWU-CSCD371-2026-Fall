using System.Globalization;
using System.Text;

namespace Dag;

/// <summary>
/// Stores directed dependencies without allowing cycles. Node identity uses
/// <see cref="EqualityComparer{T}.Default"/>, including value equality for records.
/// Values must keep the same equality and hash code while stored in the graph.
/// </summary>
public class Dag<T> where T : notnull
{
    private Dictionary<T, HashSet<T>> Dependencies { get; } = [];
    private Dictionary<T, HashSet<T>> Dependents { get; } = [];

    /// <returns>True if the node was added; false if an equal node already exists.</returns>
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

    /// <summary>
    /// Adds the edge <paramref name="dependsOn"/> → <paramref name="dependent"/>.
    /// Both nodes must already exist. A rejected dependency leaves the graph unchanged.
    /// </summary>
    /// <returns>True if the edge was added; false if it already exists.</returns>
    /// <exception cref="ArgumentException">Either node has not been added.</exception>
    /// <exception cref="InvalidOperationException">The edge would create a cycle.</exception>
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

        // A new edge closes a cycle only if its destination can already reach its source.
        if (HasPath(dependent, dependsOn))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        dependents.Add(dependent);
        dependencies.Add(dependsOn);
        return true;
    }

    /// <summary>
    /// Returns a copy of the node's direct prerequisites, in no guaranteed order.
    /// </summary>
    /// <exception cref="ArgumentException">The node has not been added.</exception>
    public T[] GetDependencies(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Dependencies.TryGetValue(value, out HashSet<T>? dependencies)
            ? [.. dependencies]
            : throw new ArgumentException("Add the node to the graph first.", nameof(value));
    }

    /// <summary>
    /// Returns a copy of the nodes that directly depend on this node, in no guaranteed order.
    /// </summary>
    /// <exception cref="ArgumentException">The node has not been added.</exception>
    public T[] GetDependents(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Dependents.TryGetValue(value, out HashSet<T>? dependents)
            ? [.. dependents]
            : throw new ArgumentException("Add the node to the graph first.", nameof(value));
    }

    /// <summary>Removes the node and all of its incoming and outgoing edges.</summary>
    /// <returns>True if the node was removed; false if it was not in the graph.</returns>
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

    /// <summary>
    /// Returns Mermaid flowchart syntax with arrows from prerequisites to dependents.
    /// Labels use the values' string representations; generated IDs keep equal labels distinct.
    /// Node and edge order is not guaranteed.
    /// </summary>
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
        // Mermaid requires at least one character inside a quoted node label.
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
