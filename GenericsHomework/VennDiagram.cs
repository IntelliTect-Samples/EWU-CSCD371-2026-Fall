namespace GenericsHomework;

/// <summary>
/// Represents a Venn diagram composed of multiple circles
/// </summary>
public class VennDiagram<T> where T : class
{
    //Hold all circles
    private readonly List<Circle<T>> _circles = new();

    //Add a new circle
    public void AddCircle(Circle<T> circle) => _circles.Add(circle);

    //Get intersection among any number of circles by name
    public IEnumerable<T> GetIntersection(params string[] circleNames)
    {
        //Get all circles whose names match given list
        var selectedCircles = _circles
            .Where(c => circleNames.Contains(c.Name))
            .ToList();

        //If there are no matching circles, return an empty list
        if (selectedCircles.Count == 0) return Enumerable.Empty<T>();

        //Start with items from the first circle
        var sharedItems = new HashSet<T>(selectedCircles[0].GetItems());

        //Compare with the rest of the circles
        foreach (var circle in selectedCircles)
        {
            sharedItems.IntersectWith(circle.GetItems());
        }

        return sharedItems;
    }

    //Get all items
    public IEnumerable<T> GetUnion(params string[] circleNames)
    {
        var selectedCircles = _circles
            .Where(c => circleNames.Contains(c.Name))
            .ToList();

        if(selectedCircles.Count == 0) return Enumerable.Empty<T>();
        var union = new HashSet<T>(selectedCircles[0].GetItems());

        foreach (var circle in selectedCircles)
        {
            union.UnionWith(circle.GetItems());
        }
        return union;
    }

}



