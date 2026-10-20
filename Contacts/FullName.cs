using System;
using System.Collections.Generic;
using System.Text;

namespace Contacts;

public sealed record FullName(
    string First,
    string Last,
    string? Middle = null)
{
    public bool Equals(FullName? other)
    {
        return other is not null
            && string.Equals(First, other.First,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(Last, other.Last,
                StringComparison.OrdinalIgnoreCase)
            && string.Equals(Middle, other.Middle,
                StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(First),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Last),
            Middle is null ? 0 :
                StringComparer.OrdinalIgnoreCase.GetHashCode(Middle));
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Middle))
        {
            return $"{First} {Last}";
        }

        return $"{First} {Middle} {Last}";
    }
}