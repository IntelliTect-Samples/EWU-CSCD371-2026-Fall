using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts;

public sealed record Employee(
    FullName Name,
    string Email,
    string Department) : IContact
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool Equals(Employee? other)
    {
        return other is not null
            && Name == other.Name
            && StringComparer.OrdinalIgnoreCase.Equals(
                Email, other.Email)
            && StringComparer.OrdinalIgnoreCase.Equals(
                Department, other.Department);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            Name,
            StringComparer.OrdinalIgnoreCase.GetHashCode(Email),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Department));
    }
}