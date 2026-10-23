using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class ContactCliTests
{
    private static readonly string[] AddAda = ["1", "Ada", "", "Lovelace", "ada@example.com", "EWU"];

    private static (ContactBook Book, string Output) Run(params string[][] commands)
    {
        ContactBook book = new();
        string script = string.Join('\n', commands.SelectMany(c => c)) + "\n";
        using StringReader input = new(script);
        using StringWriter output = new(CultureInfo.InvariantCulture);

        new ContactCli(book, input, output).Run();

        return (book, output.ToString());
    }

    private static string[] Lines(params string[] lines) => lines;

    [TestMethod]
    public void Exit_SaysGoodbye()
    {
        (_, string output) = Run(Lines("0"));

        StringAssert.Contains(output, "Goodbye.");
    }

    [TestMethod]
    public void AddStudent_StoresStudentAndConfirms()
    {
        (ContactBook book, string output) = Run(AddAda, Lines("0"));

        Assert.AreEqual(1, book.GetAll().Length);
        Assert.IsInstanceOfType<Student>(book.GetAll()[0]);
        StringAssert.Contains(output, "Added Student: Ada Lovelace <ada@example.com> - EWU");
    }

    [TestMethod]
    public void AddEmployee_StoresEmployeeWithMiddleName()
    {
        string[] addGrace = ["2", "Grace", "Brewster", "Hopper", "grace@navy.mil", "US Navy", "Rear Admiral"];

        (ContactBook book, string output) = Run(addGrace, Lines("0"));

        Assert.AreEqual(1, book.GetAll().Length);
        Assert.IsInstanceOfType<Employee>(book.GetAll()[0]);
        StringAssert.Contains(output, "Employee: Grace Brewster Hopper <grace@navy.mil> - Rear Admiral at US Navy");
    }

    [TestMethod]
    public void AddDuplicate_IsReportedAndNotStored()
    {
        (ContactBook book, string output) = Run(AddAda, AddAda, Lines("0"));

        Assert.AreEqual(1, book.GetAll().Length);
        StringAssert.Contains(output, "already exists");
    }

    [TestMethod]
    public void AddStudentAndEmployeeWithSameName_StoresBoth()
    {
        string[] addAdaEmployee = ["2", "Ada", "", "Lovelace", "ada@example.com", "EWU", "Engineer"];

        (ContactBook book, string output) = Run(AddAda, addAdaEmployee, Lines("0"));

        Assert.AreEqual(2, book.GetAll().Length);
        Assert.IsFalse(output.Contains("already exists", StringComparison.Ordinal));
    }

    [TestMethod]
    public void List_NoContacts_SaysSo()
    {
        (_, string output) = Run(Lines("3", "0"));

        StringAssert.Contains(output, "There are no contacts.");
    }

    [TestMethod]
    public void List_ShowsContactsSortedByLastName()
    {
        string[] addGrace = ["1", "Grace", "", "Hopper", "grace@navy.mil", "Vassar"];

        (_, string output) = Run(addGrace, AddAda, Lines("3", "0"));

        StringAssert.Contains(output, "1. Student: Grace Hopper");
        StringAssert.Contains(output, "2. Student: Ada Lovelace");
    }

    [TestMethod]
    public void Remove_ValidNumber_RemovesContact()
    {
        (ContactBook book, string output) = Run(AddAda, Lines("4", "1", "0"));

        Assert.AreEqual(0, book.GetAll().Length);
        StringAssert.Contains(output, "Removed Student: Ada Lovelace");
    }

    [TestMethod]
    public void Remove_NoContacts_SaysSo()
    {
        (_, string output) = Run(Lines("4", "0"));

        StringAssert.Contains(output, "There are no contacts to remove.");
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("2")]
    [DataRow("-1")]
    [DataRow("abc")]
    public void Remove_InvalidNumber_IsReportedAndKeepsContact(string number)
    {
        (ContactBook book, string output) = Run(AddAda, Lines("4", number, "0"));

        Assert.AreEqual(1, book.GetAll().Length);
        StringAssert.Contains(output, "Please enter a number from the list.");
    }

    [TestMethod]
    public void Remove_BlankInput_CancelsAndKeepsContact()
    {
        (ContactBook book, _) = Run(AddAda, Lines("4", "", "0"));

        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    [DataRow("x")]
    [DataRow("9")]
    [DataRow("")]
    public void UnknownMenuChoice_IsReportedWithoutCrashing(string choice)
    {
        (_, string output) = Run(Lines(choice, "0"));

        StringAssert.Contains(output, "Unknown option.");
        StringAssert.Contains(output, "Goodbye.");
    }

    [TestMethod]
    public void BlankRequiredValue_IsAskedAgain()
    {
        string[] blankFirstNameThenAda = ["1", "", "Ada", "", "Lovelace", "ada@example.com", "EWU"];

        (ContactBook book, string output) = Run(blankFirstNameThenAda, Lines("0"));

        Assert.AreEqual(1, book.GetAll().Length);
        StringAssert.Contains(output, "A value is required.");
    }

    [TestMethod]
    public void InvalidEmail_IsReportedWithoutCrashing()
    {
        string[] badEmail = ["1", "Ada", "", "Lovelace", "not-an-email", "EWU"];

        (ContactBook book, string output) = Run(badEmail, Lines("0"));

        Assert.AreEqual(0, book.GetAll().Length);
        StringAssert.Contains(output, "Invalid input:");
        StringAssert.Contains(output, "Goodbye.");
    }

    [TestMethod]
    public void InputEndingMidCommand_ExitsWithoutCrashing()
    {
        (ContactBook book, _) = Run(Lines("1", "Ada"));

        Assert.AreEqual(0, book.GetAll().Length);
    }

    [TestMethod]
    public void InputEndingAtMenu_ExitsWithoutCrashing()
    {
        ContactBook book = new();
        using StringReader input = new(string.Empty);
        using StringWriter output = new(CultureInfo.InvariantCulture);

        new ContactCli(book, input, output).Run();

        StringAssert.Contains(output.ToString(), "Goodbye.");
    }
}