namespace Logger;

public class Storage
{
    private HashSet<IEntity> Entities { get; } = [];
    
    public void Add(IEntity item)
    {
        Entities.Add(item);
    }

    public void Remove(IEntity item)
    {
        Entities.Remove(item);
    }

    public bool Contains(IEntity item)
    {
        return Entities.Contains(item);
    }

    public IEntity? Get(Guid id) => Entities.FirstOrDefault(e => e.Id == id);
}
