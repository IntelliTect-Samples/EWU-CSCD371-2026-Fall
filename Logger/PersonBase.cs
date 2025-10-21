using Logger;
using System;
namespace Logger;
public abstract record class PersonBase(Guid id, FullName fullName) : EntityBase
{
    public new Guid Id { get; init; } = id;
    public FullName FullName { get; init; } = fullName;
    public override string Name => FullName.Middle is null
        ? $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";
}
