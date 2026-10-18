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

        StringAssert.Contains(output, "1) Ada Lovelace - ada@example.com");
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
}
