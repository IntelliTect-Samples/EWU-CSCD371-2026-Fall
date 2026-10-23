using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Contacts.Tests;

[TestClass]
public class CLITests
{


    [TestMethod]
    public void EnterRemoveFromListValid()
    {
        // Arrange
        ContactBook book = new();
        StringWriter output = new();
        StringReader input= new("0\n");
        book.Add(new Employee(new FullName("Bob", "Smith", null), "b@x.com", "Acme", "Dev", null));
        CLI cli = new(book, input, output);

        // Act
        cli.EnterRemoveFromList();

        // Assert
        Assert.IsEmpty(book.GetAll());
        StringAssert.Contains(output.ToString(), "removed");
    }

    [TestMethod]
    public void PromptUserTestRepromptsTillValid()
    {
        //Arrange
        string testPrompt = "this is a prompt";
        string testPrompt2 = "this is a second prompt";
        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new("\n\nhello");
        CLI cli = new(book, input, output);

        //Act
        string? result = cli.PromptUser(testPrompt, testPrompt2);

        //Assert
        Assert.AreEqual("hello", result);
        StringAssert.Contains(output.ToString(), testPrompt);
        StringAssert.Contains(output.ToString(), testPrompt2);
        int v = Regex.Count(output.ToString(), Regex.Escape(testPrompt2));
        int count = v;
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void PromptUserValid()
    {
        //Arrange
        string testPrompt = "this is a prompt";
        string testPrompt2 = "this is a second prompt";
        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new("hello\n");
        CLI cli = new(book, input, output);

        //Act
        string? result = cli.PromptUser(testPrompt, testPrompt2);

        //Assert
        Assert.AreEqual("hello", result);
        StringAssert.Contains(output.ToString(), "this is a prompt");
        Assert.DoesNotContain(output.ToString(), "this is a second prompt");
    }

    [TestMethod]
    public void PromptUserNoReprompt()
    {
        //Arrange
        string testPrompt = "this is a prompt";
        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new("\n");
        CLI cli = new(book, input, output);

        //Act
        string? result = cli.PromptUser(testPrompt);

        //Assert
        Assert.AreEqual("", result);
        StringAssert.Contains(output.ToString(), "this is a prompt");
    }

    [TestMethod]
    public void EnterNameTestNoMiddleName()
    {
        //Arrange

        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new("John\n\nSmith");
        CLI cli = new(book, input, output);

        //Act
        FullName result = cli.EnterName();

        //Assert
        Assert.AreEqual("John", result.firstName);
        Assert.AreEqual("Smith", result.lastName);
        Assert.IsNull(result.middleName);
    }

    [TestMethod]
    public void EnterEmployeeContactTest()
    {
        //Arrange
        string firstName = "John";
        string lastName = "Smith";
        string email = "a@b.com";
        string company = "Cool Company";
        string jobTitle = "Test Employee";

        string inString = firstName + "\n\n" 
            + lastName + "\n"
            + email + "\n"
            + company + "\n" 
            + jobTitle + "\n\n\n";

        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new(inString);

        CLI cli = new(book, input, output);

        //Act

        var result = cli.EnterEmployeeContact();

        //Assert
        Assert.AreEqual(firstName, result.Name.firstName);
        Assert.AreEqual(lastName, result.Name.lastName);
        Assert.AreEqual(company, result.Company);
        Assert.AreEqual(jobTitle, result.JobTitle);
        Assert.IsNull(result.Phone);
    }

    [TestMethod]
    public void EnterStudentContactTest()
    {
        //Arrange
        string firstName = "John";
        string lastName = "Smith";
        string email = "a@b.com";
        string school = "Cool Company";
        string major = "Test Employee";

        string inString = firstName + "\n\n"
            + lastName + "\n"
            + email + "\n"
            + school + "\n"
            + major + "\n\n\n";

        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new(inString);

        CLI cli = new(book, input, output);

        //Act

        var result = cli.EnterStudentContact();

        //Assert
        Assert.AreEqual(firstName, result.Name.firstName);
        Assert.AreEqual(lastName, result.Name.lastName);
        Assert.AreEqual(school, result.School);
        Assert.AreEqual(major, result.Major);
        Assert.IsNull(result.Phone);
    }

    [TestMethod]
    public void EnterContactTestEmployee()
    {
        //Arrange
        string type = "Employee";
        string firstName = "John";
        string lastName = "Smith";
        string email = "a@b.com";
        string company = "Cool Company";
        string jobTitle = "Test Employee";

        string inString = "\n" 
            + type + "\n" 
            + firstName + "\n\n"
            + lastName + "\n"
            + email + "\n"
            + company + "\n"
            + jobTitle + "\n\n\n";

        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new(inString);

        CLI cli = new(book, input, output);

        //Act

        var result = cli.EnterContact();

        //Assert
        Assert.IsInstanceOfType<Employee>(result);
    }

    [TestMethod]
    public void EnterContactTestStudent()
    {
        //Arrange
        string type = "Student";
        string firstName = "John";
        string lastName = "Smith";
        string email = "a@b.com";
        string school = "Cool Company";
        string major = "Test Employee";

        string inString = "\n" 
            + type + "\n"
            + firstName + "\n\n"
            + lastName + "\n"
            + email + "\n"
            + school + "\n"
            + major + "\n\n\n";

        ContactBook book = new();
        StringWriter output = new();
        StringReader input = new(inString);

        CLI cli = new(book, input, output);

        //Act
        var result = cli.EnterContact();

        //Assert
        StringAssert.Contains(output.ToString(), "Invalid input! Try again...");
        Assert.IsInstanceOfType<Student>(result);
    }
}

