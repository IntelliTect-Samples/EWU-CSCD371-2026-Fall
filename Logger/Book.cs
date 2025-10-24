namespace Logger;

public record Book(Guid Id, string InputName): BaseEntity(Id)
{
    // Name is implemented implicitly because name should be an obvious functionality of Book
    // Though Id is not being directly implemented in this class, it is using BaseEntities implicit implementation
    public override string Name =>
        string.IsNullOrWhiteSpace(InputName) ? string.Empty : InputName;
}