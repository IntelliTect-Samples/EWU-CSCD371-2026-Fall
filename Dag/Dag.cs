namespace Dag;

public class Dag<T> where T : notnull
{
    // KEY = dependency node
    // Value = nodes that directly depend on the key
    private readonly Dictionary<T, HashSet<T>> _graph = [];
    
    /// <summary>
    /// Adds the specified node to the graph.
    /// </summary>
    /// 
    /// <param name="value">
    /// The value of the node to add.
    /// </param>
    /// 
    /// <returns>
    /// <see langword="true"/> if the node was added to the graph.
    /// <see langword="false"/> if the node already exists.
    /// </returns>
    public bool AddNode(T value) => _graph.TryAdd(value, []);

    /// <summary>
    /// Adds a directed dependency between two existing nodes.
    /// </summary>
    /// 
    /// <param name="dependsOn">
    /// The prerequisite node.
    /// </param>
    /// 
    /// <param name="dependent">
    /// The node that depends on the prerequisite.
    /// </param>
    /// 
    /// <returns>
    /// <see langword="true"/> if the dependency was added.
    /// <see langword="false"/> if either node does not exist, or the dependency already exists.
    /// </returns>
    /// 
    /// <exception cref="InvalidOperationException">
    /// Thrown when adding the dependency would create a cycle.
    /// </exception>
    public bool AddDependency(T dependsOn, T dependent)
    {
        if (!_graph.TryGetValue(dependsOn, out HashSet<T>? dependents) || !_graph.ContainsKey(dependent))
            return false;
        
        if (WouldCreateCycle(dependsOn, dependent))
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        
        return dependents.Add(dependent);
    }

    /// <summary>
    /// Gets the nodes that directly depend on the specified node.
    /// </summary>
    /// 
    /// <param name="value">
    /// The node whose dependents are to be returned
    /// </param>
    /// 
    /// <returns>
    /// A collection containing the nodes that directly depend on <paramref name="value"/>.
    /// Returns an empty collection if the node has no dependents or does not exist in the graph.
    /// </returns>
    public IReadOnlyCollection<T> GetDependents(T value)
    {
        if (!_graph.TryGetValue(value, out HashSet<T>? dependents))
            return [];
        
        return dependents.ToArray();
    }

    /// <summary>
    /// Gets the nodes that the specified node directly depends on.
    /// </summary>
    /// 
    /// <param name="value">
    /// The node whose dependencies are to be returned
    /// </param>
    /// 
    /// <returns>
    /// A collection containing the nodes that <paramref name="value"/> directly depends on.
    /// Returns an empty collection if the node has no dependencies or does not exist in the graph.
    /// </returns>
    public IReadOnlyCollection<T> GetDependencies(T value)
    {
        List<T> dependencies = [];

        foreach ((T node, HashSet<T> dependents) in _graph)
        {
            if (dependents.Contains(value))
                dependencies.Add(node);
        }
        return dependencies.ToArray();
    }

    /// <summary>
    /// Removes the specified node and all associated dependencies.
    /// </summary>
    /// 
    /// <param name="value">
    /// The node to remove.
    /// </param>
    /// 
    /// <returns>
    /// <see langword="true"/> if the node was removed.
    /// <see langword="false"/> if the node does not exist.
    /// </returns>
    public bool RemoveNode(T value)
    {
        if (!_graph.Remove(value))
            return false;

        foreach (HashSet<T> dependents in _graph.Values)
        {
            dependents.Remove(value);
        }
        return true;
    }

    /// <summary>
    /// Determines whether the graph contains the specified node.
    /// </summary>
    /// 
    /// <param name="value">
    /// The node to locate in the graph
    /// </param>
    /// 
    /// <returns>
    /// <see langword="true"/> if the node exists in the graph.
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsNode(T value) => _graph.ContainsKey(value);

    /// <summary>
    /// Renders the graph as a Mermaid flowchart.
    /// </summary>
    ///
    /// <returns>
    /// A Mermaid flowchart with one node declaration for each graph node and one
    /// directed edge for each dependency.
    /// </returns>
    public string ToMermaidFlowchart()
    {
        List<string> lines = ["flowchart TD"];
        Dictionary<T, string> nodeIds = [];

        foreach (T node in _graph.Keys)
        {
            string nodeId = $"node{nodeIds.Count}";
            nodeIds.Add(node, nodeId);

            string label = node.ToString()
                ?? throw new InvalidOperationException("Node values must return a non-null string from ToString.");
            lines.Add($"{nodeId}[\"{EscapeMermaidLabel(label)}\"]");
        }

        foreach ((T dependsOn, HashSet<T> dependents) in _graph)
        {
            foreach (T dependent in dependents)
            {
                lines.Add($"{nodeIds[dependsOn]} --> {nodeIds[dependent]}");
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    #region Helpers

    // Prevent label characters from being interpreted as Mermaid syntax.
    private static string EscapeMermaidLabel(string label)
    {
        System.Text.StringBuilder escaped = new();

        foreach (char character in label)
        {
            switch (character)
            {
                case '#':
                    escaped.Append("#35;");
                    break;
                case '&':
                    escaped.Append("#38;");
                    break;
                case '"':
                    escaped.Append("#34;");
                    break;
                case '<':
                    escaped.Append("#60;");
                    break;
                case '>':
                    escaped.Append("#62;");
                    break;
                case '\\':
                    escaped.Append("#92;");
                    break;
                default:
                    escaped.Append(char.IsControl(character) ? ' ' : character);
                    break;
            }
        }

        return escaped.ToString();
    }

    private bool WouldCreateCycle(T dependsOn, T dependent)
    {
        return CanReach(dependent, dependsOn);
    }

    private bool CanReach(T start, T target)
    {
        HashSet<T> visited = [];
        Stack<T> stack = [];

        stack.Push(start);
        
        while (stack.Count > 0)
        {
            T current = stack.Pop();

            // Found target
            if (EqualityComparer<T>.Default.Equals(current, target))
                return true;

            // Already explored
            if (!visited.Add(current))
                continue;

            // Explore outgoing edges
            foreach (T dependent in _graph[current]) 
                stack.Push(dependent);
        }

        return false;
    }

    #endregion
}