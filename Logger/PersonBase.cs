using Logger;
using System;
namespace Logger;
public abstract record class PersonBase: EntityBase
{
    public new Guid Id { get; init; }
    public FullName FullName { get; init; }

    protected PersonBase(Guid id, FullName fullName)
    {
        Id = id;
        FullName = fullName;
    }
    public override string Name => FullName.Middle is null
        ? $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";
}
