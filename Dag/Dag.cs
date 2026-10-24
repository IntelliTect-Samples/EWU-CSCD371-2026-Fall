namespace Dag;

public class Dag<T> where T : notnull
{
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
        if (!_graph.ContainsKey(dependsOn) || !_graph.ContainsKey(dependent))
            return false;
        
        // TODO: check cycle creation here - Throw error if true
        
        return _graph[dependsOn].Add(dependent);
    }
}