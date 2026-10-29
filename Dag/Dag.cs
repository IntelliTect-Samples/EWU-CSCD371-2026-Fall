using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;

namespace Dag;

public class Dag<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> _dependents = new();

    /// <summary>
    /// Adds a node to the graph if it does not already exist.
    /// </summary>
    /// <param name="value">The value of the node to add.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if value is null.
    /// </exception>
    public void AddNode(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!_dependents.ContainsKey(value))
        {
            _dependents.Add(value, new HashSet<T>());
        }
    }

    /// <summary>
    /// Adds a directed dependency from the prerequisite node to the dependent node.
    /// </summary>
    /// <param name="dependsOn">The node that must occur first.</param>
    /// <param name="dependent">The node that depends on dependsOn.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if either node does not exist in the graph.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either node value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if adding the dependency would create a cycle.
    /// </exception>
    public void AddDependency(T dependsOn, T dependent)
    {
        if (dependsOn is null)
        {
            throw new ArgumentNullException(nameof(dependsOn));
        }

        if (dependent is null)
        {
            throw new ArgumentNullException(nameof(dependent));
        }

        if (!_dependents.TryGetValue(dependsOn, out HashSet<T>? dependents))
        {
            throw new ArgumentException("Prerequisite node does not exist in the graph!");
        }

        if (!_dependents.ContainsKey(dependent))
        {
            throw new ArgumentException("The dependent node does not exist in the graph!", nameof(dependent));
        }

        if (dependents.Contains(dependent))
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

        dependents.Add(dependent);
    }

    /// <summary>
    /// Gets the nodes that the specified node directly depends on.
    /// </summary>
    /// <param name="value">The node whose dependencies are requested.</param>
    /// <returns>A list of nodes that value directly depends on.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the node does not exist in the graph.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if value is null.
    /// </exception>
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

    /// <summary>
    /// Gets the nodes that directly depend on the specified node.
    /// </summary>
    /// <param name="value">The node whose dependents are requested.</param>
    /// <returns>A list of nodes that directly depend on value.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the node does not exist in the graph.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if value is null.
    /// </exception>
    public List<T> GetDependents(T value)
    {
        if (!_dependents.TryGetValue(value, out HashSet<T>? dependents))
        {
            throw new ArgumentException("Node does not exist.", nameof(value));
        }

        return new List<T>(dependents);
    }

    /// <summary>
    /// Removes a node and all incoming and outgoing dependencies associated with it.
    /// Does nothing if the node does not exist.
    /// </summary>
    /// <param name="value">The value of the node to remove.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if value is null.
    /// </exception>
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

    /// <summary>
    /// Converts the graph into Mermaid flowchart syntax.
    /// </summary>
    /// <returns>A string representing the graph as a Mermaid flowchart.</returns>
    public string ToMermaid()
    {
        StringBuilder result = new();
        Dictionary<T, string> nodeIds = new();

        result.AppendLine("flowchart TD");

        int id = 0;

        foreach (T node in _dependents.Keys)
        {
            nodeIds.Add(node, $"node{id}");

            string label = Convert.ToString(node, CultureInfo.InvariantCulture) ?? "";
            label = label.Replace("\"", "#quot;")
                         .Replace("\r", " ")
                         .Replace("\n", " ");

            result.AppendLine(
                CultureInfo.InvariantCulture,
                $"      {nodeIds[node]}[\"{label}\"]");

            id++;
        }

        foreach (KeyValuePair<T, HashSet<T>> pair in _dependents)
        {
            foreach (T dependent in pair.Value)
            {
                result.AppendLine(
                    CultureInfo.InvariantCulture,
                    $"      {nodeIds[pair.Key]} --> {nodeIds[dependent]}");
            }
        }

        return result.ToString();
    }
}