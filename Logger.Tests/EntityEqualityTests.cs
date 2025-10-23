using Logger.Entities.Books;
using Logger.Entities.People;
using Xunit;

namespace Logger.Tests;

public class EntityEqualityTests
{
    [Fact]
    public void Books_WithSameTitleAuthorAndId_AreEqual()
    {
        Book book1 = new("1984", new FullName("George", null, "Orwell"));
        Book book2 = book1 with { }; // clone book1

        Assert.Equal(book1, book2);
        Assert.True(book1 == book2);
        Assert.False(book1 != book2);
    }

    [Fact]
    public void Books_WithDifferentIds_AreNotEqual()
    {
        Book book1 = new("1984", new FullName("George", null, "Orwell"));
        Book book2 = new("1984", new FullName("George", null, "Orwell"));

        Assert.NotEqual(book1, book2);
        Assert.True(book1 != book2);
    }

    [Fact]
    public void Books_WithDifferentAuthors_AreNotEqual()
    {
        Book book1 = new("1984", new FullName("George", null, "Orwell"));
        Book book2 = new("1984", new FullName("George", "Jr.", "Orwell"));

        Assert.NotEqual(book1, book2);
        Assert.True(book1 != book2);
    }

    [Fact]
    public void Students_WithDifferentIds_AreNotEqual()
    {
        FullName fullName = new("Jane", "A.", "Doe");
        Student s1 = new(fullName, 3.9m);
        Student s2 = new(fullName, 3.9m);

        Assert.NotEqual(s1, s2);
    }

    [Fact]
    public void Students_WithDifferentGPA_AreNotEqual()
    {
        FullName name = new("Jane", null, "Doe");
        var s1 = new Student(name, 3.5m);
        var s2 = new Student(name, 2.5m);

        Assert.NotEqual(s1, s2);
    }

    [Fact]
    public void Employees_WithDifferentIds_AreNotEqual()
    {
        FullName fullName = new("John", "Q", "Public");
        Employee e1 = new(fullName, 50000);
        Employee e2 = new(fullName, 50000);

        Assert.NotEqual(e1, e2);
    }

    [Fact]
    public void Employees_WithDifferentSalary_AreNotEqual()
    {
        FullName fullName = new("John", "Q", "Public");
        Employee e1 = new(fullName, 50000);
        Employee e2 = new(fullName, 60000);

        Assert.NotEqual(e1, e2);
    }

    [Fact]
    public void Book_StudentOrEmployee_AreNotEqual()
    {
        Book book = new("1984", new FullName("George", null, "Orwell"));
        Student student = new(new FullName("George", null, "Orwell"), 3.5m);
        Employee employee = new(new FullName("George", null, "Orwell"), 50000);

        Assert.False(book.Equals(student));
        Assert.False(book.Equals(employee));
    }
}

