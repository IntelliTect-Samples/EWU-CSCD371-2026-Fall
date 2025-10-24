using System;

namespace Logger;
//FullName represents a person's full name with first, last, and optional middle name
//It is defined as a record because it is data container
//We want equality based on its values rather than the object's references

//This type is immutable
//Immutability ensures that once FullName instance is created, its data cannot
//accidentally change, maintaining data consistency
public record FullName
    {
        public string First { get; init; }
        public string Last { get; init; }
        public string? Middle { get; init; }

        public FullName(string first, string last, string? middle = null)
        {
            if (string.IsNullOrWhiteSpace(first))
                throw new ArgumentException("First name cannot be null or whitespace.", nameof(first));

            if (string.IsNullOrWhiteSpace(last))
                throw new ArgumentException("Last name cannot be null or whitespace.", nameof(last));

            First = first.Trim();
            Last = last.Trim();
            Middle = string.IsNullOrWhiteSpace(middle) ? null : middle.Trim();
    }
    }


