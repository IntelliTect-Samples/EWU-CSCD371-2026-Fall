using Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
[DoNotParallelize]
public class ProgramTests
{
    private const string StudentEntry =
        "1\n" +
        "Alex\n" +
        "Smith\n" +
        "\n" +
        "Computer Science\n" +
        "2028\n" +
        "alex@example.com\n" +
        "\n";

    private const string EmployeeEntry =
        "2\n" +
        "Jamie\n" +
        "Rivera\n" +
        "Morgan\n" +
        "Engineering\n" +
        "Contoso\n" +
        "jamie@example.com\n" +
        "j.rivera@example.com\n" +
        "\n";

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"Contacts.ProgramTests.{Guid.NewGuid():N}");

        Directory.CreateDirectory(path);
        return path;
    }

    private static string RunProgram(
        string input,
        ContactBook? contactBook = null)
    {
        string? temporaryDirectory = null;
        var originalDirectory = Environment.CurrentDirectory;

        if (contactBook is null)
        {
            temporaryDirectory = CreateTempDirectory();
            Environment.CurrentDirectory = temporaryDirectory;
        }

        var originalInput = Console.In;
        var originalOutput = Console.Out;

        using var reader = new StringReader(input);
        using var writer = new StringWriter();

        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            if (contactBook is null)
                Program.Main();
            else
                Program.Run(contactBook);

            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
            Environment.CurrentDirectory = originalDirectory;

            if (temporaryDirectory is not null
                && Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }

    [TestMethod]
    public void Main_InvalidMenuOption_ReportsErrorAndContinues()
    {
        var output = RunProgram("invalid\n6\n");

        StringAssert.Contains(
            output,
            "Invalid option. Please try again.");
    }

    [TestMethod]
    public void Main_EndOfInput_ExitsWithoutLooping()
    {
        var output = RunProgram(string.Empty);

        StringAssert.Contains(
            output,
            "Input ended. Saving contacts and exiting.");
    }

    [TestMethod]
    public void Main_AddsStudentAndReportsDuplicate()
    {
        var output = RunProgram(
            StudentEntry + StudentEntry + "6\n");

        StringAssert.Contains(output, "Student added.");
        StringAssert.Contains(
            output,
            "Duplicate student - not added.");
    }

    [TestMethod]
    public void Main_AddsEmployeeAndReportsDuplicate()
    {
        var output = RunProgram(
            EmployeeEntry + EmployeeEntry + "6\n");

        StringAssert.Contains(output, "Employee added.");
        StringAssert.Contains(
            output,
            "Duplicate employee - not added.");
    }

    [TestMethod]
    public void Main_ListContacts_WhenEmpty_ReportsNoContacts()
    {
        var output = RunProgram("3\n6\n");

        StringAssert.Contains(output, "No contacts found.");
    }

    [TestMethod]
    public void Main_ListContacts_DisplaysStudentsAndEmployees()
    {
        var output = RunProgram(
            StudentEntry + EmployeeEntry + "3\n6\n");

        StringAssert.Contains(output, "Contacts:");
        StringAssert.Contains(output, "Student | ID:");
        StringAssert.Contains(output, "Name: Alex Smith");
        StringAssert.Contains(output, "Major: Computer Science");
        StringAssert.Contains(output, "Graduation year: 2028");
        StringAssert.Contains(output, "Employee | ID:");
        StringAssert.Contains(output, "Name: Jamie Morgan Rivera");
        StringAssert.Contains(output, "Department: Engineering");
        StringAssert.Contains(output, "Company: Contoso");
        StringAssert.Contains(output, "j.rivera@example.com");
    }

    [TestMethod]
    public void Main_ListContacts_ContactWithoutEmailsDisplaysNone()
    {
        var directory = CreateTempDirectory();

        try
        {
            var book = new ContactBook(
                Path.Combine(directory, "contacts.json"));

            Assert.IsTrue(
                book.Add(
                    new Student(
                        new FullName("Alex", "Smith", null),
                        "Computer Science",
                        "2028",
                        Array.Empty<string>())));

            var output = RunProgram("3\n6\n", book);

            StringAssert.Contains(output, "Emails: (none)");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void Main_RemoveContact_WhenEmpty_ReportsNoContacts()
    {
        var output = RunProgram("4\n6\n");

        StringAssert.Contains(output, "No contacts found.");
    }

    [TestMethod]
    public void Main_RemoveContact_InvalidSelectionsAreRejected()
    {
        var input =
            StudentEntry +
            "4\nnot-a-number\n" +
            "4\n0\n" +
            "4\n99\n" +
            "4\n1\n" +
            "6\n";

        var output = RunProgram(input);

        var invalidCount =
            output.Split(
                "Invalid selection. No contact was changed.",
                StringSplitOptions.None).Length - 1;

        Assert.AreEqual(3, invalidCount);
        StringAssert.Contains(output, "Contact removed.");
    }

    [TestMethod]
    public void Main_RemoveContact_RemovesSelectedContact()
    {
        var output = RunProgram(
            StudentEntry + "4\n1\n3\n6\n");

        StringAssert.Contains(output, "Contact removed.");
        StringAssert.Contains(output, "No contacts found.");
    }

    [TestMethod]
    public void Main_EditStudent_PreservesIdAndChangesDetails()
    {
        var directory = CreateTempDirectory();

        try
        {
            var book = new ContactBook(
                Path.Combine(directory, "contacts.json"));

            var original = new Student(
                new FullName("Alex", "Smith", null),
                "Computer Science",
                "2028",
                ["alex@example.com"]);

            Assert.IsTrue(book.Add(original));

            var input =
                "5\n1\n" +
                "Alex\nSmith\n\n" +
                "Mathematics\n" +
                "2029\n" +
                "alex.new@example.com\n\n" +
                "3\n6\n";

            var output = RunProgram(input, book);
            var edited = book.GetAll().OfType<Student>().Single();

            Assert.AreEqual(original.ID, edited.ID);
            Assert.AreEqual("Mathematics", edited.Major);
            Assert.AreEqual("2029", edited.GraduationYear);
            Assert.AreEqual(1, edited.Emails.Count);
            Assert.AreEqual("alex.new@example.com", edited.Emails[0]);

            StringAssert.Contains(output, "Contact updated.");
            StringAssert.Contains(output, "Major: Mathematics");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void Main_EditEmployee_PreservesIdAndChangesDetails()
    {
        var directory = CreateTempDirectory();

        try
        {
            var book = new ContactBook(
                Path.Combine(directory, "contacts.json"));

            var original = new Employee(
                new FullName("Jamie", "Rivera", "Morgan"),
                "Engineering",
                "Contoso",
                ["jamie@example.com"]);

            Assert.IsTrue(book.Add(original));

            var input =
                "5\n1\n" +
                "Jamie\nRivera\nMorgan\n" +
                "Security\n" +
                "Fabrikam\n" +
                "jamie.new@example.com\n\n" +
                "6\n";

            var output = RunProgram(input, book);
            var edited = book.GetAll().OfType<Employee>().Single();

            Assert.AreEqual(original.ID, edited.ID);
            Assert.AreEqual("Security", edited.Department);
            Assert.AreEqual("Fabrikam", edited.CompanyName);
            Assert.AreEqual(1, edited.Emails.Count);
            Assert.AreEqual("jamie.new@example.com", edited.Emails[0]);

            StringAssert.Contains(output, "Contact updated.");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void Main_EditContact_RejectingDuplicatePreservesOriginal()
    {
        var directory = CreateTempDirectory();

        try
        {
            var book = new ContactBook(
                Path.Combine(directory, "contacts.json"));

            var first = new Student(
                new FullName("Alex", "Smith", null),
                "Computer Science",
                "2028",
                ["alex@example.com"]);

            var second = new Student(
                new FullName("Alex", "Smith", null),
                "Mathematics",
                "2028",
                ["alex@example.com"]);

            Assert.IsTrue(book.Add(first));
            Assert.IsTrue(book.Add(second));

            // Select the first contact shown by this book.
            var displayedContacts = book.GetAll();
            var selected = (Student)displayedContacts[0];
            var duplicateTarget = (Student)displayedContacts[1];

            var middleName = duplicateTarget.name.Middle ?? string.Empty;
            var emails = string.Join("\n", duplicateTarget.Emails);

            var input =
                "5\n1\n" +
                $"{duplicateTarget.name.First}\n" +
                $"{duplicateTarget.name.Last}\n" +
                $"{middleName}\n" +
                $"{duplicateTarget.Major}\n" +
                $"{duplicateTarget.GraduationYear}\n" +
                $"{emails}\n\n" +
                "6\n";

            var output = RunProgram(input, book);

            StringAssert.Contains(
                output,
                "Edit rejected because it would create a duplicate.");

            Assert.AreEqual(2, book.GetAll().Length);
            Assert.IsTrue(book.Contains(selected));
            Assert.IsTrue(book.Contains(duplicateTarget));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}