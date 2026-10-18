namespace Contacts.Tests;

[TestClass]
public class ProgramTests
{
    private static string RunWith(ContactBook book, params string[] lines)
    {
        StringWriter output = new();

        Program.Run(book, new StringReader(string.Join('\n', lines) + "\n"), output);

        return output.ToString();
    }

    private static string[] AddStudent() => ["1", "Ada", "", "Lovelace", "ada@example.com", "CS", ""];

    private static string[] AddEmployee() => ["2", "Ada", "", "Lovelace", "ada@example.com", "CS", ""];

    [TestMethod]
    public void Run_Quit_ExitsAfterShowingMenu()
    {
        string output = RunWith(new ContactBook(), "0");

        StringAssert.Contains(output, "0) Quit");
    }

    [TestMethod]
    public void Run_AddStudent_StoresStudent()
    {
        ContactBook book = new();

        string output = RunWith(book, [.. AddStudent(), "0"]);

        StringAssert.Contains(output, "Contact added.");
        Assert.IsInstanceOfType<Student>(book.GetAll().Single());
    }

    [TestMethod]
    public void Run_AddDuplicate_ReportsDuplicate()
    {
        ContactBook book = new();

        string output = RunWith(book, [.. AddStudent(), .. AddStudent(), "0"]);

        StringAssert.Contains(output, "That contact already exists.");
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Run_StudentAndEmployeeWithSameData_BothStored()
    {
        ContactBook book = new();

        RunWith(book, [.. AddStudent(), .. AddEmployee(), "0"]);

        Assert.HasCount(2, book.GetAll());
    }

    [TestMethod]
    public void Run_BlankFieldsAndBadChoice_DoNotCrash()
    {
        ContactBook book = new();

        string output = RunWith(book, "1", "", "", "", "abc", "0");

        StringAssert.Contains(output, "Invalid input");
        StringAssert.Contains(output, "Invalid choice.");
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Run_List_ShowsContacts()
    {
        string output = RunWith(new ContactBook(), [.. AddStudent(), "3", "0"]);

        StringAssert.Contains(output, "1) Ada Lovelace - ada@example.com (Student, CS)");
    }

    [TestMethod]
    public void Run_RemoveValidNumber_RemovesContact()
    {
        ContactBook book = new();

        string output = RunWith(book, [.. AddStudent(), "4", "1", "0"]);

        StringAssert.Contains(output, "Contact removed.");
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Run_RemoveInvalidNumber_KeepsContact()
    {
        ContactBook book = new();

        string output = RunWith(book, [.. AddStudent(), "4", "99", "4", "abc", "0"]);

        StringAssert.Contains(output, "Invalid number.");
        Assert.HasCount(1, book.GetAll());
    }

    private static FullName Ada => new("Ada", "Lovelace");

    private static string NumberOf(ContactBook book, IContact contact) =>
        (Array.IndexOf(book.GetAll(), contact) + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

    [TestMethod]
    public void Run_NullArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => Program.Run(null!, new StringReader(""), new StringWriter()));
        Assert.Throws<ArgumentNullException>(() => Program.Run(new ContactBook(), null!, new StringWriter()));
        Assert.Throws<ArgumentNullException>(() => Program.Run(new ContactBook(), new StringReader(""), null!));
    }

    [TestMethod]
    public void Run_EndOfInput_ExitsWithoutQuit()
    {
        StringWriter output = new();

        Program.Run(new ContactBook(), new StringReader(""), output);

        StringAssert.Contains(output.ToString(), "Choose: ");
    }

    [TestMethod]
    public void Run_EndOfInputWhileAdding_ExitsCleanly()
    {
        ContactBook book = new();
        StringWriter output = new();

        Program.Run(book, new StringReader("1\nAda\n"), output);

        Assert.AreEqual(1, output.ToString().Split("Choose: ").Length - 1);
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Run_AddEmployee_StoresEmployee()
    {
        ContactBook book = new();

        string output = RunWith(book, "2", "Grace", "Brewster", "Hopper", "grace@example.com, grace@navy.mil", "Admiral", "555-0100", "0");

        StringAssert.Contains(output, "Contact added.");
        Employee employee = (Employee)book.GetAll().Single();
        Assert.AreEqual(new FullName("Grace", "Hopper", "Brewster"), employee.Name);
        CollectionAssert.AreEqual(new[] { "grace@example.com", "grace@navy.mil" }, employee.Emails.ToArray());
        Assert.AreEqual("Admiral", employee.JobTitle);
        Assert.AreEqual("555-0100", employee.Phone);
    }

    [TestMethod]
    public void Run_AddEmployeeWithBlankFields_ReportsInvalidInput()
    {
        ContactBook book = new();

        string output = RunWith(book, "2", "", "", "", "", "", "", "0");

        StringAssert.Contains(output, "Invalid input");
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Run_AddWithNoEmails_ReportsInvalidInput()
    {
        ContactBook book = new();

        string output = RunWith(book, "1", "Ada", "", "Lovelace", " , ", "CS", "", "0");

        StringAssert.Contains(output, "At least one email is required.");
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Run_ListEmptyBook_ShowsNoContacts()
    {
        string output = RunWith(new ContactBook(), "3", "0");

        StringAssert.Contains(output, "No contacts.");
    }

    [TestMethod]
    public void Run_ListMultipleEmails_ShowsAllEmails()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, ["ada@example.com", "ada@ewu.edu"], "CS"));

        string output = RunWith(book, "3", "0");

        StringAssert.Contains(output, "1) Ada Lovelace - ada@example.com, ada@ewu.edu (Student, CS)");
    }

    [TestMethod]
    public void Run_ListStudentAndEmployeeWithSameData_ShowsEachType()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, ["ada@example.com"], "Engineer"));
        book.Add(new Employee(Ada, ["ada@example.com"], "Engineer"));

        string output = RunWith(book, "3", "0");

        StringAssert.Contains(output, "Ada Lovelace - ada@example.com (Student, Engineer)");
        StringAssert.Contains(output, "Ada Lovelace - ada@example.com (Employee, Engineer)");
    }

    [TestMethod]
    public void Run_RemoveFromEmptyBook_ShowsNoContacts()
    {
        string output = RunWith(new ContactBook(), "4", "0");

        StringAssert.Contains(output, "No contacts.");
    }

    [TestMethod]
    public void Run_EditEmptyBook_ShowsNoContacts()
    {
        string output = RunWith(new ContactBook(), "5", "0");

        StringAssert.Contains(output, "No contacts.");
    }

    [TestMethod]
    public void Run_EditInvalidNumber_ReportsInvalidNumber()
    {
        ContactBook book = new();
        Student student = new(Ada, ["ada@example.com"], "CS");
        book.Add(student);

        string output = RunWith(book, "5", "0", "5", "2", "5", "abc", "0");

        StringAssert.Contains(output, "Invalid number.");
        Assert.AreSame(student, book.GetAll().Single());
    }

    [TestMethod]
    public void Run_EditStudentKeepingEverything_KeepsValuesAndId()
    {
        ContactBook book = new();
        Student student = new(new FullName("Ada", "Lovelace", "King"), ["ada@example.com"], "CS", "555-0100");
        book.Add(student);

        string output = RunWith(book, "5", "1", "", "", "", "", "", "", "0");

        StringAssert.Contains(output, "Contact updated.");
        StringAssert.Contains(output, "Middle name [King]: ");
        Student edited = (Student)book.GetAll().Single();
        Assert.AreEqual(student, edited);
        Assert.AreEqual(student.Id, edited.Id);
        Assert.AreEqual("King", edited.Name.MiddleName);
        Assert.AreEqual("555-0100", edited.Phone);
    }

    [TestMethod]
    public void Run_EditStudentChangingFields_UpdatesContactAndKeepsId()
    {
        ContactBook book = new();
        Student student = new(new FullName("Ada", "Lovelace", "King"), ["ada@example.com"], "CS", "555-0100");
        book.Add(student);

        string output = RunWith(book, "5", "1", "Augusta", "-", "Byron", "new@example.com, other@example.com", "Math", "555-0199", "0");

        StringAssert.Contains(output, "Contact updated.");
        Student edited = (Student)book.GetAll().Single();
        Assert.AreEqual(student.Id, edited.Id);
        Assert.AreEqual(new FullName("Augusta", "Byron"), edited.Name);
        CollectionAssert.AreEqual(new[] { "new@example.com", "other@example.com" }, edited.Emails.ToArray());
        Assert.AreEqual("Math", edited.Major);
        Assert.AreEqual("555-0199", edited.Phone);
    }

    [TestMethod]
    public void Run_EditEmployee_UpdatesJobTitleAndClearsPhone()
    {
        ContactBook book = new();
        Employee employee = new(new FullName("Grace", "Hopper"), ["grace@example.com"], "Captain", "555-0100");
        book.Add(employee);

        string output = RunWith(book, "5", "1", "", "Brewster", "", "", "Admiral", "-", "0");

        StringAssert.Contains(output, "Contact updated.");
        StringAssert.Contains(output, "Job title [Captain]: ");
        Employee edited = (Employee)book.GetAll().Single();
        Assert.AreEqual(employee.Id, edited.Id);
        Assert.AreEqual(new FullName("Grace", "Hopper", "Brewster"), edited.Name);
        Assert.AreEqual("Admiral", edited.JobTitle);
        Assert.IsNull(edited.Phone);
    }

    [TestMethod]
    public void Run_EditIntoDuplicate_ReportsDuplicateAndKeepsOriginal()
    {
        ContactBook book = new();
        Student ada = new(Ada, ["ada@example.com"], "CS");
        Student grace = new(new FullName("Grace", "Hopper"), ["grace@example.com"], "CS");
        book.Add(ada);
        book.Add(grace);

        string output = RunWith(book, "5", NumberOf(book, grace), "Ada", "", "Lovelace", "ada@example.com", "", "", "0");

        StringAssert.Contains(output, "That contact already exists.");
        Assert.HasCount(2, book.GetAll());
        Assert.IsTrue(book.Contains(ada));
        Assert.IsTrue(book.Contains(grace));
    }

    [TestMethod]
    public void Run_EditWithInvalidEmails_ReportsInvalidInputAndKeepsOriginal()
    {
        ContactBook book = new();
        Student student = new(Ada, ["ada@example.com"], "CS");
        book.Add(student);

        string output = RunWith(book, "5", "1", "", "", "", ",", "", "", "0");

        StringAssert.Contains(output, "Invalid input");
        Assert.AreSame(student, book.GetAll().Single());
    }

    [TestMethod]
    public void Run_EditUnknownContactType_Throws()
    {
        ContactBook book = new();
        book.Add(new OtherContact(Guid.NewGuid(), Ada, ["ada@example.com"], null));

        Assert.Throws<InvalidOperationException>(() => RunWith(book, "5", "1", "0"));
    }

    private sealed record OtherContact(Guid Id, FullName Name, IReadOnlyList<string> Emails, string? Phone) : IContact;

    [TestMethod]
    public void Load_MissingFile_ReturnsEmptyBookWithoutMessage()
    {
        StringWriter output = new();

        ContactBook book = Program.Load(TempPath(), output);

        Assert.IsEmpty(book.GetAll());
        Assert.AreEqual("", output.ToString());
    }

    [TestMethod]
    public void Load_CorruptFile_ReportsAndReturnsEmptyBook()
    {
        string path = TempPath();
        File.WriteAllText(path, "{ not json");
        StringWriter output = new();

        try
        {
            ContactBook book = Program.Load(path, output);

            Assert.IsEmpty(book.GetAll());
            StringAssert.Contains(output.ToString(), "Could not load contacts, starting empty");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void Load_InvalidContactData_ReportsAndReturnsEmptyBook()
    {
        string path = TempPath();
        File.WriteAllText(path, """[{"Type":"Student","Name":{"FirstName":"A","LastName":"B"},"Emails":[],"Major":"CS"}]""");
        StringWriter output = new();

        try
        {
            ContactBook book = Program.Load(path, output);

            Assert.IsEmpty(book.GetAll());
            StringAssert.Contains(output.ToString(), "At least one email is required.");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void Load_PathIsDirectory_ReturnsEmptyBook()
    {
        Assert.IsEmpty(Program.Load(Path.GetTempPath(), new StringWriter()).GetAll());
    }

    [TestMethod]
    public void SaveThenLoad_RoundTripsContacts()
    {
        string path = TempPath();
        ContactBook book = new();
        Student student = new(Ada, ["ada@example.com"], "CS");
        book.Add(student);
        StringWriter output = new();

        try
        {
            Program.Save(book, path, output);
            ContactBook loaded = Program.Load(path, output);

            Assert.IsTrue(loaded.Contains(student));
            Assert.AreEqual("", output.ToString());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void Save_MissingDirectory_ReportsError()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "contacts.json");
        StringWriter output = new();

        Program.Save(new ContactBook(), path, output);

        StringAssert.Contains(output.ToString(), "Could not save contacts");
    }

    [TestMethod]
    public void LoadAndSave_NullOutput_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => Program.Load(TempPath(), null!));
        Assert.Throws<ArgumentNullException>(() => Program.Save(new ContactBook(), TempPath(), null!));
    }

    [TestMethod]
    public void Main_LoadsRunsAndSavesContactsFile()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "contacts.json");
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        ContactBook saved = new();
        saved.Add(new Student(Ada, ["ada@example.com"], "CS"));
        ContactStorage.Save(saved, path);
        StringWriter output = new();

        try
        {
            Console.SetIn(new StringReader(string.Join('\n', AddEmployee()) + "\n3\n0\n"));
            Console.SetOut(output);

            Program.Main();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        try
        {
            StringAssert.Contains(output.ToString(), "Ada Lovelace - ada@example.com");
            StringAssert.Contains(output.ToString(), "Contact added.");
            Assert.HasCount(2, ContactStorage.Load(path).GetAll());
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string TempPath() => Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid()}.json");
}
