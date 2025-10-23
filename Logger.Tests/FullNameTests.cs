using Xunit;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void Constructor_AllNames_InitializesProperties()
    {
        var name = new FullName("John", "Paul", "Jones");

        Assert.Equal("John", name.First);
        Assert.Equal("Paul", name.Middle);
        Assert.Equal("Jones", name.Last);
    }

    [Fact]
    public void Constructor_FirstIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FullName(null!, "X", "Y"));
    }

    [Fact]
    public void Constructor_LastIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FullName("X", "Y", null!));
    }

    [Theory]
    [InlineData("John", "Paul", "Jones", "John Paul Jones")]
    [InlineData("John", null, "Jones", "John Jones")]
    [InlineData("John", "", "Jones", "John Jones")]
    [InlineData("John", "   ", "Jones", "John Jones")]
    public void ToString_ValidNames_ProperlyFormatted(string first, string? middle, string last, string expected)
    {
        FullName name = new(first, middle, last);
        Assert.Equal(expected, name.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Middle_EmptyOrWhitespace_IsNull(string middle)
    {
        FullName name = new("A", middle, "B");

        Assert.Null(name.Middle);
    }

    [Fact]
    public void Equality_ValidFullNames_UsesValueEquality()
    {
        FullName name1 = new("John", "Paul", "Jones");
        FullName name2 = new("John", "Paul", "Jones");
        FullName name3 = new("John", "David", "Jones");

        Assert.Equal(name1, name2);
        Assert.NotEqual(name1, name3);
    }
}
