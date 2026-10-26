using System;
using System.Collections.Generic;
using System.Text;

namespace Dag;

public class Dag<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependents = new();

    public void AddNode(T value)
    {
        if (!_dependents.ContainsKey(value))
        {
            _dependents.Add(value, new HashSet<T>());
        }
    }

    public void AddDependency(T dependsOn, T dependent)
    {
        if (!_dependents.ContainsKey(dependsOn) ||
            !_dependents.ContainsKey(dependent))
        {
            throw new ArgumentException("Both nodes must exist in the graph.");
        }

        if (_dependents[dependsOn].Contains(dependent))
        {
            return;
        }

        Stack<T> stack = new();
        HashSet<T> visited = new();
        stack.Push(dependent);

        while (stack.Count > 0)
        {
            T current = stack.Pop();

            if (EqualityComparer<T>.Default.Equals(current, dependsOn))
            {
                throw new InvalidOperationException(
                    "Adding this dependency would create a cycle.");
            }

            if (visited.Add(current))
            {
                foreach (T next in _dependents[current])
                {
                    stack.Push(next);
                }
            }
        }

        _dependents[dependsOn].Add(dependent);
    }

    public List<T> GetDependencies(T value)
    {
        if (!_dependents.ContainsKey(value))
        {
            throw new ArgumentException("Node does not exist.", nameof(value));
        }

        List<T> dependencies = new();

        foreach (KeyValuePair<T, HashSet<T>> pair in _dependents)
        {
            if (pair.Value.Contains(value))
            {
                dependencies.Add(pair.Key);
            }
        }

        return dependencies;
    }

    public List<T> GetDependents(T value)
    {
        if (!_dependents.ContainsKey(value))
        {
            throw new ArgumentException("Node does not exist.", nameof(value));
        }

        return new List<T>(_dependents[value]);
    }

    public void RemoveNode(T value)
    {
        if (!_dependents.ContainsKey(value))
        {
            return;
        }

        _dependents.Remove(value);

        foreach (HashSet<T> dependents in _dependents.Values)
        {
            dependents.Remove(value);
        }
    }

    public string ToMermaid()
    {
        StringBuilder result = new();
        Dictionary<T, string> nodeIds = new();

        result.AppendLine("flowchart TD");

        int id = 0;

        foreach (T node in _dependents.Keys)
        {
            nodeIds.Add(node, $"node{id}");

            string label = Convert.ToString(node) ?? "";
            label = label.Replace("\"", "#quot;")
                         .Replace("\r", " ")
                         .Replace("\n", " ");

            result.AppendLine($"    {nodeIds[node]}[\"{label}\"]");

            id++;
        }

        foreach (KeyValuePair<T, HashSet<T>> pair in _dependents)
        {
            foreach (T dependent in pair.Value)
            {
                result.AppendLine(
                    $"    {nodeIds[pair.Key]} --> {nodeIds[dependent]}");
            }
        }

        return result.ToString();
    }
}