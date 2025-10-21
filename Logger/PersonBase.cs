using Logger;
using System;
namespace Logger;

public abstract record class PersonBase: EntityBase
{
    /// <summary>
    /// Explicitly redefines ID using new keyword to avoid conflicts with EntityBase.Id
    /// We do this to ensure that PersonBase has its own unique identifier separate from EntityBase
    /// This ensures person based entities share consistent internal logic
    /// </summary>
    public new Guid Id { get; init; }
    /// <summary>
    /// Fullname is exposed as a normal public memeber
    /// </summary>
    public FullName FullName { get; init; }
    /// <summary>
    /// Implements Name Implicitly so that i remains part of the public API
    ///Name is computed from FullName rather than being stored
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fullName"></param>
    protected PersonBase(Guid id, FullName fullName)
    {
        Id = id;
        FullName = fullName;
    }
    public override string Name => FullName.Middle is null
        ? $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";
}
