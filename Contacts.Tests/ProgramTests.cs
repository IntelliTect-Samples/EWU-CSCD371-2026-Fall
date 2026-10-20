
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Contacts.Tests;
///summary
/// This is a test class for the Program class in the Contacts application.
/// /summary
[TestClass]
public class ProgramTests
{

  /// <summary>
  /// This test method verifies that the Main method of the Program class starts the application correctly. It captures the console output and checks if it contains the expected startup message.
  /// </summary>
  [TestMethod]
  public void Main_ShouldStartApplication()
  {
    // Arrange
    using var sw = new StringWriter();
    using var input = new StringReader("4");
    var originalOut = Console.Out;
    var originalIn = Console.In;

    try
    {
      Console.SetOut(sw);
      Console.SetIn(input);

      // Act
      Program.Main();

      // Assert
      var expected = "Contacts Application Started!";
      var actual = sw.ToString();
      Assert.IsTrue(actual.Contains(expected, StringComparison.Ordinal));
    }
    finally
    {
      Console.SetOut(originalOut);
      Console.SetIn(originalIn);
    }
  }


 
  /// <summary>
  /// This test method verifies that the Main method of the Program class exits the application when the user selects option 4. It captures the console output and checks if it contains the expected prompt message.
  /// </summary>
  [TestMethod]
  public void Main_ShouldExitApplicationOnOption4()
  {
    // Arrange
    using var sw = new StringWriter();
    using var input = new StringReader("4");
    var originalOut = Console.Out;
    var originalIn = Console.In;

    try
    {
      Console.SetOut(sw);
      Console.SetIn(input);

      // Act
      Program.Main();

      // Assert
      var expected = "Please select an option (1-4): ";
      var actual = sw.ToString();
      Assert.IsTrue(actual.Contains(expected, StringComparison.Ordinal));
    }
    finally
    {
      Console.SetOut(originalOut);
      Console.SetIn(originalIn);
    }
  }


  /// <summary>
  /// This test method verifies that the Main method of the Program class loops until the user selects option 4 to exit. It simulates user input for option 1 followed by option 4 and checks if the console output contains the expected prompt message.
  /// </summary>
  [TestMethod]
  public void Main_ShouldLoopUntilExit()
  {
    // Arrange
    using var sw = new StringWriter();
    using var input = new StringReader("1\n4");
    var originalOut = Console.Out;
    var originalIn = Console.In;

    try
    {
      Console.SetOut(sw);
      Console.SetIn(input);

      // Act
      Program.Main();

      // Assert
      var expected = "Please select an option (1-4): ";
      var actual = sw.ToString();
      Assert.IsTrue(actual.Contains(expected, StringComparison.Ordinal));
    }
    finally
    {
      Console.SetOut(originalOut);
      Console.SetIn(originalIn);
    }
  }


  // Student Tests  
    // Valid student data
    // Invalid student data


  // Employee Tests
    // Valid employee data
    // Invalid employee data  
}