using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts;

public sealed record Student(
    FullName Name,
    string Email,
    string Major) : IContact
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool Equals(Student? other)
    {
        return other is not null
            && Name == other.Name
            && StringComparer.OrdinalIgnoreCase.Equals(
                Email, other.Email)
            && StringComparer.OrdinalIgnoreCase.Equals(
                Major, other.Major);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Name,
            StringComparer.OrdinalIgnoreCase.GetHashCode(Email),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Major));
    }
}