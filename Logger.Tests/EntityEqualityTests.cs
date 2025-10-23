using System;
using Xunit;

namespace Logger.Tests;

public class EntityEqualityTests
{

    [Fact]
    public void Student_Equality_ByID()
    {
        Guid id = Guid.NewGuid();
        var s1 = new Student { Id = id, FullName = new ("Ada", "Lovelace", "K") };
        var s2 = new Student { Id = id, FullName = new ("Ada", "Lovelace") };

        Assert.Equal(s1, s2);
        Assert.Equal(s1.GetHashCode(), s2.GetHashCode());
    }

    [Fact]
    public void Employee_Equality_ByID()
    {
        Guid id = Guid.NewGuid();
        var e1 = new Employee { Id = id, FullName = new("Grace", "Hopper"), Department = "R&D" };
        var e2 = new Employee { Id = id, FullName = new("Grace", "Hopper", "M"), Department = "Ops" };

        Assert.Equal(e1, e2);
        Assert.Equal(e1.GetHashCode(), e2.GetHashCode());
    }

    [Fact]
    public void Book_Equality_ByID()
    {
        Guid id = Guid.NewGuid();
        var b1 = new Book { Id = id, Title = "Clean Code" };
        var b2 = new Book { Id = id, Title = "Clean Code (2nd printing)" };

        Assert.Equal(b1, b2);
        Assert.Equal(b1.GetHashCode(), b2.GetHashCode());
    }
}
