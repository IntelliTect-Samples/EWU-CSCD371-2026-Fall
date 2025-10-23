using System;
using Xunit;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void FullName_ValidParams_CreatesFullName()
    {
        FullName name = new("Inigo", "Montoya");
        Assert.NotNull(name);
        Assert.Equal("Inigo", name.First);
        Assert.Equal("Montoya", name.Last);
        Assert.Null(name.Middle);
    }

    [Fact]
    public void FullName_MiddleAbsentOrNull_InstancesAreEqual()
    {
        FullName a = new("Prince", "Humperdinck");
        FullName b = new("Prince", "Humperdinck", null);
        Assert.Equal(a, b);
    }

    [Fact]
    public void FullName_SameNamesWithMiddle_Equality()
    {
        FullName a = new("Charles", "John", "Dickens");
        FullName b = new("Charles", "John", "Dickens");
        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void FullName_WithAndWithoutMiddle_NotEqual()
    {
        FullName a = new("Stu", "Steiner");
        FullName b = new("Stu", "Steiner", "Superman");
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void FullName_CompareObjectToString_Equality()
    {
        FullName withMiddle = new("Willy", "Wonka", "Wilbur");
        Assert.Equal("Willy Wilbur Wonka", withMiddle.Name);

        FullName withoutMiddle = new("Willy", "Wonka");
        Assert.Equal("Willy Wonka", withoutMiddle.Name);
    }

    [Fact]
    public void FullName_AddingMiddleLater_NotEqualButIsChanged()
    {
        FullName original = new("Biggs", "Wedge");
        FullName modified = original with { Middle = "&" };

        Assert.NotSame(original, modified);
        Assert.Equal("&", modified.Middle);
        Assert.Null(original.Middle);
    }

    [Fact]
    public void FullName_MissingFirstLastNames_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new FullName("", "LastName"));
        Assert.Throws<ArgumentException>(() => new FullName("FirstName", "\t"));
    }
}
