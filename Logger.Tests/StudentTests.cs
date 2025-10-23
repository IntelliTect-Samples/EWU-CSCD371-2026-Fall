using System;
using Xunit;

namespace Logger.Tests;

public class StudentTests
{

    [Fact]
    public void Student_Create_InitializesProperties()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var student = new Student { Id = id, FullName = new("Lionel", "Messi"), StudentNumber = "10" };

        // Act
        var studentNumber = student.StudentNumber;
        Guid studentId = student.Id;
        var name = student.FullName;

        // Assert
        Assert.Equal("10", studentNumber);
        Assert.Equal("Lionel", name.First);
        Assert.Equal("Messi", name.Last);
        Assert.Equal("Messi, Lionel", name.ToString());
        Assert.Equal(id, studentId);
    }

    [Fact]
    public void Student_NullStudentNumber_ReturnsNull()
    {
        // Arrange
        var student = new Student { FullName = new("Paulo", "Dybala") };

        // Act
        var studentNumber = student.StudentNumber;

        // Assert
        Assert.Null(studentNumber);
    }
}