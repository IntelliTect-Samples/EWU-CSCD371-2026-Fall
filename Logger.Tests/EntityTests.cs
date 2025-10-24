using Xunit;

namespace Logger.Tests;

public class EntityTests
{
    [Fact]
    public void Equals_OneBookObject_ReturnsTrue()
    {
        Book testBook = new Book() { Title = "Test Book" };

        Assert.True(testBook.Equals(testBook));
    }

    [Fact]
    public void Equals_TwoBookObjectsWithSameTitle_ReturnsTrue()
    {
        Book book1 = new Book() { Title = "Test Book" };
        Book book2 = new Book() { Title = "Test Book" };

        Assert.True(book1.Equals(book2));
    }

    [Fact]
    public void Equals_TwoBookObjectsWithDifferentTitles_ReturnsFalse()
    {
        Book book1 = new Book() { Title = "Test Book 1" };
        Book book2 = new Book() { Title = "Test Book 2" };

        Assert.False(book1.Equals(book2));
    }

    [Fact]
    public void Equals_OneStudentObject_ReturnsTrue()
    {
        Student testStudent = new Student()
        {
            FullName = new FullName("Test", "Student"),
            StudentId = "S12345"
        };

        Assert.True(testStudent.Equals(testStudent));
    }

    [Fact]
    public void Equals_TwoStudentObjectsWithSameIdAndName_ReturnsTrue()
    {
        Student student1 = new Student()
        {
            FullName = new FullName("Test", "Student"),
            StudentId = "S12345"
        };
        Student student2 = new Student()
        {
            FullName = new FullName("Test", "Student"),
            StudentId = "S12345"
        };

        Assert.True(student1.Equals(student2));
    }

    [Fact]
    public void Equals_TwoStudentObjectsWithDifferentIds_ReturnsFalse()
    {
        Student student1 = new Student()
        {
            FullName = new FullName("Test", "Student"),
            StudentId = "S12345"
        };
        Student student2 = new Student()
        {
            FullName = new FullName("Test", "Student"),
            StudentId = "S67890"
        };

        Assert.False(student1.Equals(student2));
    }

    [Fact]
    public void Equals_TwoStudentObjectsWithDifferentNames_ReturnsFalse()
    {
        Student student1 = new Student()
        {
            FullName = new FullName("Test", "Student1"),
            StudentId = "S12345"
        };
        Student student2 = new Student()
        {
            FullName = new FullName("Test", "Student2"),
            StudentId = "S12345"
        };

        Assert.False(student1.Equals(student2));
    }

    [Fact]
    public void Equals_OneEmployeeObject_ReturnsTrue()
    {
        Employee testEmployee = new Employee()
        {
            FullName = new FullName("Test", "Employee"),
            Position = "E12345"
        };

        Assert.True(testEmployee.Equals(testEmployee));
    }

    [Fact]
    public void Equals_TwoEmployeeObjectsWithSamePositionAndName_ReturnsTrue()
    {
        Employee employee1 = new Employee
        {
            FullName = new FullName("Test", "Employee"),
            Position = "E12345"
        };
        Employee employee2 = new Employee
        {
            FullName = new FullName("Test", "Employee"),
            Position = "E12345"
        };

        Assert.True(employee1.Equals(employee2));
    }

    [Fact]
    public void Equals_TwoEmployeeObjectsWithDifferentPositions_ReturnsFalse()
    {
        Employee employee1 = new Employee
        {
            FullName = new FullName("Test", "Employee"),
            Position = "E12345"
        };
        Employee employee2 = new Employee
        {
            FullName = new FullName("Test", "Employee"),
            Position = "E67890"
        };

        Assert.False(employee1.Equals(employee2));
    }

    [Fact]
    public void Equals_TwoEmployeeObjectsWithDifferentNames_ReturnsFalse()
    {
        Employee employee1 = new Employee
        {
            FullName = new FullName("Test", "Employee1"),
            Position = "E12345"
        };
        Employee employee2 = new Employee
        {
            FullName = new FullName("Test", "Employee2"),
            Position = "E12345"
        };

        Assert.False(employee1.Equals(employee2));
    }
}