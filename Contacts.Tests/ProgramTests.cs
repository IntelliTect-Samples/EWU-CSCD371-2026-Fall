using System;
using System.IO;
using System.Linq;
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
            "1", "Alice", "Smith", "", "alice@example.com, alice@school.edu", "", "", "", "",
            "1", "Alice", "Smith", "", "alice@school.edu, alice@example.com", "", "", "", "",
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
            contactBook.Add(new Student(new FullName("Alice", "Smith"), ["alice@example.com"]));
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
            contactBook.Add(new Employee(new FullName("Bob", "Jones"), ["bob@example.com"]));
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

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EditContact_UpdatesFieldsAndPreservesId(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "Ann", "alice@example.com, alice@school.edu", "555-0100", "Old address",
                "Old organization", "Old role", "0");
            IContact original = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());

            string output = RunWithStoragePath(storagePath,
                "5", "1", "Alicia", "Johnson", "-", "updated@example.com, other@example.com", "-", "New address",
                "New organization", "New role", "3", "0");

            Contact expected = menuOption == "1"
                ? new Student(new FullName("Alicia", "Johnson"), ["updated@example.com", "other@example.com"], address: "New address",
                    schoolName: "New organization", major: "New role")
                : new Employee(new FullName("Alicia", "Johnson"), ["updated@example.com", "other@example.com"], address: "New address",
                    employer: "New organization", jobTitle: "New role");
            IContact updated = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());

            StringAssert.Contains(output, "Contact updated.");
            StringAssert.Contains(output, "Name: Alicia Johnson");
            Assert.AreEqual(expected, updated);
            Assert.AreEqual(original.Id, updated.Id);
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EditContactWithBlankFields_KeepsValuesAndId(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "Ann", "alice@example.com, alice@school.edu", "555-0100", "Old address",
                "Old organization", "Old role", "0");
            IContact original = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());

            string output = RunWithStoragePath(storagePath,
                "5", "1", "", "", "", "", "", "", "", "", "0");
            IContact updated = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());

            StringAssert.Contains(output, "Contact updated.");
            Assert.AreEqual(original, updated);
            Assert.AreEqual(original.Id, updated.Id);
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EditContactWithDashes_ClearsOptionalFields(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "Ann", "alice@example.com, alice@school.edu", "555-0100", "Old address",
                "Old organization", "Old role", "0");

            string output = RunWithStoragePath(storagePath,
                "5", "1", "", "", "-", "-", "", "-", "-", "-", "0");

            Contact expected = menuOption == "1"
                ? new Student(new FullName("Alice", "Smith"), phoneNumber: "555-0100")
                : new Employee(new FullName("Alice", "Smith"), phoneNumber: "555-0100");
            IContact updated = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());

            StringAssert.Contains(output, "Contact updated.");
            Assert.AreEqual(expected, updated);
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EditIntoDuplicate_ReportsDuplicateAndPreservesContacts(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", "alice@example.com, alice@school.edu", "", "", "", "",
                menuOption, "Bob", "Jones", "", "bob@example.com, bob@school.edu", "", "", "", "", "0");
            IContact[] originals = ContactBook.LoadFromFile(storagePath).GetAll();
            string originalJson = File.ReadAllText(storagePath);

            string output = RunWithStoragePath(storagePath,
                "5", "2", "Alice", "Smith", "", "alice@school.edu, alice@example.com", "", "", "", "", "3", "0");

            StringAssert.Contains(output, "A contact with the same information already exists.");
            Assert.AreEqual(2, output.Split("Name: Alice Smith").Length - 1);
            Assert.AreEqual(2, output.Split("Name: Bob Jones").Length - 1);
            Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));

            IContact[] stored = ContactBook.LoadFromFile(storagePath).GetAll();
            Assert.HasCount(2, stored);
            foreach (IContact original in originals)
            {
                Assert.AreEqual(original, Array.Find(stored, contact => contact.Id == original.Id));
            }
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_EditClearingAllContactMethods_ReportsErrorAndKeepsOriginal(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", "alice@example.com, alice@school.edu", "", "", "", "", "0");
            string originalJson = File.ReadAllText(storagePath);

            string output = RunWithStoragePath(storagePath,
                "5", "1", "", "", "", "-", "", "", "", "", "3", "0");

            StringAssert.Contains(output, "Could not edit contact:");
            Assert.AreEqual(2, output.Split("Emails: alice@example.com, alice@school.edu").Length - 1);
            Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    public void Run_EditStudentToMatchEmployee_StoresBothTypes()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "",
                "2", "Bob", "Jones", "", "bob@example.com", "", "", "", "", "0");

            string output = RunWithStoragePath(storagePath,
                "5", "1", "Bob", "Jones", "", "bob@example.com", "", "", "", "", "0");
            ContactBook contactBook = ContactBook.LoadFromFile(storagePath);

            StringAssert.Contains(output, "Contact updated.");
            Assert.HasCount(2, contactBook.GetAll());
            Assert.IsTrue(contactBook.Contains(new Student(new FullName("Bob", "Jones"), ["bob@example.com"])));
            Assert.IsTrue(contactBook.Contains(new Employee(new FullName("Bob", "Jones"), ["bob@example.com"])));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    public void Run_EditEmptyContactBook_ReportsNoContacts()
    {
        string output = RunWithInput("5", "0");

        StringAssert.Contains(output, "No contacts to edit.");
        Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow("not a number")]
    [DataRow("0")]
    [DataRow("2")]
    public void Run_EditInvalidContactNumber_ReportsErrorAndKeepsOriginal(string selection)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                "1", "Alice", "Smith", "", "alice@example.com", "", "", "", "", "0");
            string originalJson = File.ReadAllText(storagePath);

            string output = RunWithStoragePath(storagePath, "5", selection, "0");

            StringAssert.Contains(output, "Invalid contact number.");
            Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1", 1)]
    [DataRow("1", 7)]
    [DataRow("2", 7)]
    public void Run_EndOfInputWhileEditing_ExitsWithoutChangingSavedContacts(string menuOption, int completedFields)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", "alice@example.com", "", "", "", "", "0");
            string originalJson = File.ReadAllText(storagePath);
            string[] lines = ["5", "1", "Alicia", "Johnson", "", "updated@example.com", "", "", "New organization", "New role"];

            string output = RunWithStoragePath(storagePath, lines[..(completedFields + 2)]);

            StringAssert.Contains(output, "Goodbye.");
            Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_SaveFailsWhenEditing_ReportsErrorAndKeepsOriginal(string menuOption)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"contacts-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string storagePath = Path.Combine(directory, "contacts.json");
        string originalPath = Path.Combine(directory, "original.json");

        try
        {
            RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", "alice@example.com, alice@school.edu", "", "", "", "", "0");
            string originalJson = File.ReadAllText(storagePath);
            Guid originalId = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll()).Id;
            using BeforeFirstReadReader input = new("5\n1\nAlicia\n\n\n\n\n\n\n\n3\n0", () =>
            {
                File.Move(storagePath, originalPath);
                Directory.CreateDirectory(storagePath);
            });
            using StringWriter output = new();

            Program.Run(input, output, storagePath);
            string text = output.ToString();

            StringAssert.Contains(text, "Could not save contacts:");
            Assert.AreEqual(2, text.Split("Name: Alice Smith").Length - 1);
            Assert.AreEqual(2, text.Split($"Id: {originalId}").Length - 1);
            Assert.IsFalse(text.Contains("Name: Alicia Smith", StringComparison.Ordinal));
            Assert.IsFalse(text.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(originalPath));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_AddMultipleEmails_ListsAndPersistsAllAddresses(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            string output = RunWithStoragePath(storagePath,
                menuOption, "Alice", "Smith", "", " work@example.com, personal@example.com, , work@example.com ",
                "", "", "", "", "3", "0");
            IContact contact = Assert.ContainsSingle(ContactBook.LoadFromFile(storagePath).GetAll());
            string[] expected = ["personal@example.com", "work@example.com"];

            StringAssert.Contains(output, "Contact added.");
            StringAssert.Contains(output, "Emails: personal@example.com, work@example.com");
            CollectionAssert.AreEqual(expected, contact.Emails.ToArray());
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("1")]
    [DataRow("2")]
    public void Run_AddOnlyEmptyEmailEntries_ReportsMissingContactMethod(string menuOption)
    {
        string output = RunWithInput(
            menuOption, "Alice", "Smith", "", ", ,", "", "", "", "", "3", "0");

        StringAssert.Contains(output, "At least one contact method must be entered");
        StringAssert.Contains(output, "No contacts found.");
        Assert.IsFalse(output.Contains("Contact added.", StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow("1", 0)]
    [DataRow("1", 1)]
    [DataRow("1", 2)]
    [DataRow("2", 0)]
    [DataRow("2", 1)]
    [DataRow("2", 2)]
    public void Run_EndOfInputWhileReadingName_ExitsWithoutChangingSavedContacts(string menuOption, int completedNames)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            ContactBook contactBook = new();
            contactBook.Add(new Employee(new FullName("Bob", "Jones"), ["bob@example.com"]));
            contactBook.SaveToFile(storagePath);
            string originalJson = File.ReadAllText(storagePath);
            string[] lines = [menuOption, "Alice", "Smith"];

            string output = RunWithStoragePath(storagePath, lines[..(completedNames + 1)]);

            StringAssert.Contains(output, "Goodbye.");
            Assert.IsFalse(output.Contains("Contact added.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    [DataRow("4")]
    [DataRow("5")]
    public void Run_EndOfInputWhileSelectingContact_ExitsWithoutChangingSavedContacts(string menuOption)
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            ContactBook contactBook = new();
            contactBook.Add(new Student(new FullName("Alice", "Smith"), ["alice@example.com"]));
            contactBook.SaveToFile(storagePath);
            string originalJson = File.ReadAllText(storagePath);

            string output = RunWithStoragePath(storagePath, menuOption);

            StringAssert.Contains(output, "Enter the number of the contact");
            StringAssert.Contains(output, "Goodbye.");
            Assert.IsFalse(output.Contains("Contact removed.", StringComparison.Ordinal));
            Assert.IsFalse(output.Contains("Contact updated.", StringComparison.Ordinal));
            Assert.AreEqual(originalJson, File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    public void Run_StoredJsonNull_ReportsLoadErrorAndPreservesFile()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        try
        {
            File.WriteAllText(storagePath, "null");

            string output = RunWithStoragePath(storagePath, "0");

            StringAssert.Contains(output, "Could not load contacts:");
            StringAssert.Contains(output, "The contacts file must contain a JSON array.");
            Assert.AreEqual("null", File.ReadAllText(storagePath));
        }
        finally
        {
            File.Delete(storagePath);
        }
    }

    [TestMethod]
    public void Run_InvalidUnicodeInStoredJson_ReportsLoadErrorAndPreservesFile()
    {
        string storagePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");
        string json = """
            [{
              "$type": "student",
              "Name": { "FirstName": "\uD800", "LastName": "Smith" },
              "Emails": ["alice@example.com"]
            }]
            """;

        try
        {
            File.WriteAllText(storagePath, json);

            string output = RunWithStoragePath(storagePath, "0");

            StringAssert.Contains(output, "Could not load contacts:");
            Assert.AreEqual(json, File.ReadAllText(storagePath));
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
