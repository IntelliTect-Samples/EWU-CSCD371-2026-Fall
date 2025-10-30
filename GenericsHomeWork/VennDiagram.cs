using System.Collections.Generic;
using System.Linq;

namespace GenericsHomeWork;

public class VennDiagram<T> where T : class
{
    private readonly List<Circle<T>> _circles = new List<Circle<T>>();

    public IReadOnlyList<Circle<T>> Circles => _circles;

    public void AddCircle(Circle<T> circle)
    {
        _circles.Add(circle);
    }

    public IEnumerable<T> FindIntersection(params Circle<T>[] circles)
    {
        if (circles == null || circles.Length == 0)
        {
            return Enumerable.Empty<T>();
        }

        IEnumerable<T> intersection = circles.First().GetItems();

        foreach (var circle in circles.Skip(1))
        {
            intersection = intersection.Intersect(circle.GetItems());
        }

        return intersection;
    }

    public IEnumerable<T> FindUnion()
    {
        var unionSet = new HashSet<T>();
        foreach (var circle in _circles)
        {
            unionSet.UnionWith(circle.GetItems());
        }
        return unionSet;
    }

    public IEnumerable<T> FindUniqueToCircle(Circle<T> targetCircle)
    {
        var itemsInOtherCircles = new HashSet<T>();
        foreach (var circle in _circles)
        {
            if (circle != targetCircle)
            {
                itemsInOtherCircles.UnionWith(circle.GetItems());
            }
        }
        return targetCircle.GetItems().Except(itemsInOtherCircles);
    }
}
