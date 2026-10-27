namespace Dag;

public class Dag<T> where T : notnull
{
    private readonly Dictionary<T, HashSet<T>> dag = new Dictionary<T, HashSet<T>>();
    public void AddNode(T node) // NO DUPLICATES ALLOWED
    {
        if (dag.ContainsKey(node))
        {
            return; // Node already exists, do nothing
        }
        dag.Add(node, new HashSet<T>());
    }

      public void RemoveNode(T value)
    {
        if (!dag.Remove(value))
        {
            throw new ArgumentException("Node does not exist.");
        }

        foreach (var dependents in dag.Values)
        {
            dependents.Remove(value);
        }
    }

    public string ToMermaid()
    {
        var lines = new List<string> { "flowchart TD" };
        var nodeIds = new Dictionary<T, string>();

        int index = 0;

        foreach (var node in dag.Keys)
        {
            string id = $"n{index++}";
            nodeIds[node] = id;

            string label = node.ToString() ?? "";
            label = label.Replace("\"", "&quot;");

            lines.Add($"    {id}[\"{label}\"]");
        }

        foreach (var node in dag)
        {
            foreach (var dependent in node.Value)
            {
                lines.Add(
                    $"    {nodeIds[node.Key]} --> {nodeIds[dependent]}"
                );
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    public void AddDependency(T dependsOn, T dependent) // NO CYCLES ALLOWED
    {
       if (!dag.ContainsKey(dependsOn) || !dag.ContainsKey(dependent))
        {
            throw new ArgumentException("Both nodes must exist in the DAG.");
        }

        // Check for cycles
        if (CycleCheck(dependsOn, dependent))
        {
            throw new InvalidOperationException("Adding this dependency would create a cycle.");
        }

        dag[dependsOn].Add(dependent);
    }

    public IReadOnlyCollection<T> GetDependencies(T dependent)
    {
        if (!dag.ContainsKey(dependent))
        {
            throw new ArgumentException("Node does not exist in the DAG.");
        }
        var dependencies = new HashSet<T>();
        foreach (var node in dag)
        {
            if (node.Value.Contains(dependent))
            {
                dependencies.Add(node.Key);
            }
        }
        return dependencies;
    }

    public IReadOnlyCollection<T> GetDependents(T dependsOn)
    {
        if (!dag.ContainsKey(dependsOn))
        {
            throw new ArgumentException("Node does not exist in the DAG.");
        }

        var dependents = new HashSet<T>();
        foreach (var node in dag[dependsOn])
        {
          dependents.Add(node);
        }
        return dependents;
    }

    private bool CycleCheck(T dependsOn, T dependent)
    {
        var visited = new HashSet<T>();
        var stack = new Stack<T>();
        stack.Push(dependent);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (EqualityComparer<T>.Default.Equals(current, dependsOn))
            {
                return true; // Cycle detected
            }

            if (!visited.Contains(current))
            {
                visited.Add(current);
                if (dag.ContainsKey(current))
                {
                    foreach (var neighbor in dag[current])
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        return false; // No cycle detected
    }
}