using System;

namespace Logger;

// Base class for person-based entities (Student and Employee)
// Refactors common code: both use FullName and Email
public abstract record PersonEntity : EntityBase
{
    public FullName FullName { get; init; }
    public string Email { get; init; }

    // Name is calculated from FullName properties
    // No backing field - computed on each access
    // Implemented implicitly - person names should be directly accessible
    public override string Name => FullName.Middle != null
        ? $"{FullName.First} {FullName.Middle} {FullName.Last}"
        : $"{FullName.First} {FullName.Last}";

    protected PersonEntity(Guid id, FullName fullName, string email) : base(id)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}