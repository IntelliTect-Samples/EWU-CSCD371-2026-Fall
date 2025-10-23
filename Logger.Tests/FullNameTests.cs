using System;
using Xunit;

namespace Logger.Tests;

public class FullNameTests
{

    [Fact]
    public void ToString_WithMiddleName_PrintsFullName()
    {
        // Arrange 
        var fullName = new FullName("Paul", "Pogba", "P");

        // Act
        var result = fullName.ToString();
    
        // Assert
        Assert.Equal("Pogba, Paul P.", result);
    }

    [Fact]
    public void ToString_WithoutMiddleName_PrintsFullName()
    {
        // Arrange 
        var fullName = new FullName("Kylian", "Mbappe");

        // Act
        var result = fullName.ToString();

        // Assert
        Assert.Equal("Mbappe, Kylian", result);
        Assert.Null(fullName.Middle);
    }




}