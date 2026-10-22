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

    [TestMethod]
    public void Run_LoadsContactsFromPreviousRun()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            string addOutput = RunWithStoragePath(storagePath,
                "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "", "0");
            string listOutput = RunWithStoragePath(storagePath, "3", "0");

            StringAssert.Contains(addOutput, "Contact added.");
            StringAssert.Contains(listOutput, "1. Student");
            StringAssert.Contains(listOutput, "Name: Alice Smith");
        }
        finally
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }
        }
    }

    [TestMethod]
    public void Run_PersistsContactRemoval()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "", "0");
            string removeOutput = RunWithStoragePath(storagePath, "4", "1", "0");
            string listOutput = RunWithStoragePath(storagePath, "3", "0");

            StringAssert.Contains(removeOutput, "Contact removed.");
            StringAssert.Contains(listOutput, "No contacts found.");
        }
        finally
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }
        }
    }

    [TestMethod]
    public void Run_InvalidStoredJson_ReportsLoadError()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            File.WriteAllText(storagePath, "not json");

            string output = RunWithStoragePath(storagePath, "0");

            StringAssert.Contains(output, "Could not load contacts:");
        }
        finally
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_SaveFailsWhenAdding_ReportsErrorAndRollsBack(string menuOption)
    {
        string missingDirectory = Path.Combine(Path.GetTempPath(), $"contacts-test-{Guid.NewGuid():N}");
        string storagePath = Path.Combine(missingDirectory, "contacts.json");

        string output = RunWithStoragePath(storagePath,
            menuOption, "Alice", "Smith", "", "alice@example.com", "", "", "", "",
            "3", "0");

        StringAssert.Contains(output, "Could not save contacts:");
        StringAssert.Contains(output, "No contacts found.");
        Assert.IsFalse(output.Contains("Contact added.", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Run_SaveFailsWhenRemoving_ReportsErrorAndKeepsContact()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"contacts-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string storagePath = Path.Combine(directory, "contacts.json");

        try
        {
            ContactBook contactBook = new();
            contactBook.Add(new Student(new FullName("Alice", "Smith"), "alice@example.com"));
            contactBook.SaveToFile(storagePath);

            using BeforeFirstReadReader input = new("4\n1\n3\n0", () =>
            {
                // Loading has finished; block saving without relying on file permissions.
                File.Move(storagePath, Path.Combine(directory, "original.json"));
                Directory.CreateDirectory(storagePath);
            });
            using StringWriter output = new();

            Program.Run(input, output, storagePath);
            string text = output.ToString();

            StringAssert.Contains(text, "Could not save contacts:");
            Assert.AreEqual(2, text.Split("Name: Alice Smith").Length - 1);
            Assert.IsFalse(text.Contains("Contact removed.", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EndOfInputWhileAdding_ExitsWithoutChangingSavedContacts(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            ContactBook contactBook = new();
            contactBook.Add(new Employee(new FullName("Bob", "Jones"), "bob@example.com"));
            contactBook.SaveToFile(storagePath);
            string originalJson = File.ReadAllText(storagePath);

            string output = RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", "alice@example.com");

            StringAssert.Contains(output, "Goodbye.");
            Assert.IsFalse(output.Contains("Contact added.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    private static string RunWithInput(params string[] lines)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            return RunWithStoragePath(storagePath, lines);
        }
        finally
        {
            if (File.Exists(storagePath))
            {
                File.Delete(storagePath);
            }
        }
    }

    private static string RunWithStoragePath(string storagePath, params string[] lines)
    {
        using StringReader input = new(string.Join(Environment.NewLine, lines) + Environment.NewLine);
        using StringWriter output = new();

        Program.Run(input, output, storagePath);

        return output.ToString();
    }

    private sealed class BeforeFirstReadReader(string text, Action beforeFirstRead) : StringReader(text)
    {
        private bool _hasRead;

        public override string? ReadLine()
        {
            if (!_hasRead)
            {
                beforeFirstRead();
                _hasRead = true;
            }

            return base.ReadLine();
        }
    }
}
