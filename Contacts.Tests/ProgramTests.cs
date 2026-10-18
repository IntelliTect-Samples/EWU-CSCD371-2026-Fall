using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Run_NullInput_ThrowsArgumentNullException()
    {
        using StringWriter output = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => Program.Run(null!, output));
    }

    [TestMethod]
    public void Run_NullOutput_ThrowsArgumentNullException()
    {
        using StringReader input = new("0");

        Assert.ThrowsExactly<ArgumentNullException>(() => Program.Run(input, null!));
    }

    [TestMethod]
    public void Run_EndOfInput_ExitsGracefully()
    {
        string output = RunWithInput();

        StringAssert.Contains(output, "Goodbye.");
    }

    [TestMethod]
    public void Run_AddsAndListsStudentsAndEmployees()
    {
        string output = RunWithInput(
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "EWU", "Computer Science",
            "2", "Bob", "Jones", "", "", "555-0100", "", "Acme", "Developer",
            "3", "0");

        StringAssert.Contains(output, "Contact added.");
        StringAssert.Contains(output, "1. Student");
        StringAssert.Contains(output, "Name: Alice Smith");
        StringAssert.Contains(output, "SchoolName: EWU");
        StringAssert.Contains(output, "2. Employee");
        StringAssert.Contains(output, "Name: Bob Jones");
        StringAssert.Contains(output, "Employer: Acme");
        StringAssert.Contains(output, "JobTitle: Developer");
    }

    [TestMethod]
    public void Run_ReportsDuplicateContact()
    {
        string output = RunWithInput(
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "3", "0");

        StringAssert.Contains(output, "A contact with the same information already exists.");
        Assert.AreEqual(1, output.Split("Name: Alice Smith").Length - 1);
    }

    [TestMethod]
    public void Run_RemovesSelectedContact()
    {
        string output = RunWithInput(
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "4", "1",
            "3", "0");

        StringAssert.Contains(output, "Contact removed.");
        StringAssert.Contains(output, "No contacts found.");
    }

    [TestMethod]
    public void Run_ReportsInvalidInputAndContinues()
    {
        string output = RunWithInput(
            "not an option",
            "1", "", "Alice", "", "Smith", "", "", "", "", "", "",
            "3", "4",
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "4", "not a number", "3", "0");

        StringAssert.Contains(output, "Invalid option.");
        StringAssert.Contains(output, "This field is required.");
        StringAssert.Contains(output, "Could not add student:");
        StringAssert.Contains(output, "No contacts found.");
        StringAssert.Contains(output, "No contacts to remove.");
        StringAssert.Contains(output, "Invalid contact number.");
        StringAssert.Contains(output, "Name: Alice Smith");
    }

    [TestMethod]
    public void Run_ReportsOutOfRangeContactNumber()
    {
        string output = RunWithInput(
            "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "4", "2",
            "3", "0");

        StringAssert.Contains(output, "Invalid contact number.");
        StringAssert.Contains(output, "Name: Alice Smith");
    }

    private static string RunWithInput(params string[] lines)
    {
        using StringReader input = new(string.Join(Environment.NewLine, lines) + Environment.NewLine);
        using StringWriter output = new();

        Program.Run(input, output);

        return output.ToString();
    }
}
