using System;
using Xunit;
using Logger;

namespace Logger.Tests;

public class StorageTests
{
    #region Book Entity Tests
    
    [Fact]
    public void Add_Book_ShouldAddEntityToStorage()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book(Guid.NewGuid(), "The Great Gatsby", "F. Scott Fitzgerald", "9780743273565");
        
        // Act
        storage.Add(book);
        
        // Assert
        Assert.True(storage.Contains(book));
    }
    
    [Fact]
    public void Get_Book_ShouldReturnCorrectBook()
    {
        // Arrange
        var storage = new Storage();
        var id = Guid.NewGuid();
        var book = new Book(id, "1984", "George Orwell", "9780451524935");
        storage.Add(book);
        
        // Act
        var result = storage.Get(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(book, result);
        Assert.Equal("1984", ((Book)result).Title);
    }
    
    #endregion
    
    #region Student Entity Tests
    
    [Fact]
    public void Add_Student_ShouldAddEntityToStorage()
    {
        // Arrange
        var storage = new Storage();
        var student = new Student(
            Guid.NewGuid(),
            new FullName("John", "Doe", "Michael"),
            "john.doe@example.com");
        
        // Act
        storage.Add(student);
        
        // Assert
        Assert.True(storage.Contains(student));
    }
    
    [Fact]
    public void Get_Student_ShouldReturnCorrectStudent()
    {
        // Arrange
        var storage = new Storage();
        var id = Guid.NewGuid();
        var student = new Student(
            id,
            new FullName("Jane", "Smith"),
            "jane.smith@example.com");
        storage.Add(student);
        
        // Act
        var result = storage.Get(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(student, result);
        Assert.Equal("Jane Smith", result.Name);
    }
    
    #endregion
    
    #region Employee Entity Tests
    
    [Fact]
    public void Add_Employee_ShouldAddEntityToStorage()
    {
        // Arrange
        var storage = new Storage();
        var employee = new Employee(
            Guid.NewGuid(),
            new FullName("Alice", "Johnson", "Marie"),
            "alice.johnson@company.com",
            "Software Engineer");
        
        // Act
        storage.Add(employee);
        
        // Assert
        Assert.True(storage.Contains(employee));
    }
    
    [Fact]
    public void Get_Employee_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var storage = new Storage();
        var id = Guid.NewGuid();
        var employee = new Employee(
            id,
            new FullName("Bob", "Williams"),
            "bob.williams@company.com",
            "Manager");
        storage.Add(employee);
        
        // Act
        var result = storage.Get(id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(employee, result);
        Assert.Equal("Bob Williams", result.Name);
    }
    
    #endregion
    
    #region Mixed Entity Tests
    
    [Fact]
    public void Add_MultipleEntityTypes_ShouldAddAllToStorage()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book(Guid.NewGuid(), "Clean Code", "Robert Martin", "9780132350884");
        var student = new Student(
            Guid.NewGuid(),
            new FullName("Charlie", "Brown"),
            "charlie.brown@example.com");
        var employee = new Employee(
            Guid.NewGuid(),
            new FullName("Diana", "Prince"),
            "diana.prince@company.com",
            "Director");
        
        // Act
        storage.Add(book);
        storage.Add(student);
        storage.Add(employee);
        
        // Assert
        Assert.True(storage.Contains(book));
        Assert.True(storage.Contains(student));
        Assert.True(storage.Contains(employee));
    }
    
    [Fact]
    public void Get_WithMultipleEntityTypes_ShouldReturnCorrectEntity()
    {
        // Arrange
        var storage = new Storage();
        var bookId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        var book = new Book(bookId, "Design Patterns", "Gang of Four", "9780201633610");
        var student = new Student(
            studentId,
            new FullName("Eve", "Taylor"),
            "eve.taylor@example.com");
        var employee = new Employee(
            employeeId,
            new FullName("Frank", "Moore"),
            "frank.moore@company.com",
            "Analyst");
        
        storage.Add(book);
        storage.Add(student);
        storage.Add(employee);
        
        // Act
        var retrievedBook = storage.Get(bookId);
        var retrievedStudent = storage.Get(studentId);
        var retrievedEmployee = storage.Get(employeeId);
        
        // Assert
        Assert.NotNull(retrievedBook);
        Assert.NotNull(retrievedStudent);
        Assert.NotNull(retrievedEmployee);
        Assert.IsType<Book>(retrievedBook);
        Assert.IsType<Student>(retrievedStudent);
        Assert.IsType<Employee>(retrievedEmployee);
        Assert.Equal(book, retrievedBook);
        Assert.Equal(student, retrievedStudent);
        Assert.Equal(employee, retrievedEmployee);
    }
    
    #endregion
    
    #region Remove Tests
    
    [Fact]
    public void Remove_Book_ShouldRemoveFromStorage()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book(Guid.NewGuid(), "Test Book", "Test Author", "1234567890");
        storage.Add(book);
        
        // Act
        storage.Remove(book);
        
        // Assert
        Assert.False(storage.Contains(book));
    }
    
    [Fact]
    public void Remove_Student_ShouldRemoveFromStorage()
    {
        // Arrange
        var storage = new Storage();
        var student = new Student(
            Guid.NewGuid(),
            new FullName("Remove", "Test"),
            "remove.test@example.com");
        storage.Add(student);
        
        // Act
        storage.Remove(student);
        
        // Assert
        Assert.False(storage.Contains(student));
    }
    
    [Fact]
    public void Remove_NonExistentEntity_ShouldNotThrow()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book(Guid.NewGuid(), "Non Existent", "Author", "0000000000");
        
        // Act & Assert
        storage.Remove(book); // Should not throw
        Assert.False(storage.Contains(book));
    }
    
    #endregion
    
    #region Duplicate and Equality Tests
    
    [Fact]
    public void Add_DuplicateBook_ShouldNotAddTwice()
    {
        // Arrange
        var storage = new Storage();
        var id = Guid.NewGuid();
        var book1 = new Book(id, "Duplicate Test", "Author One", "1111111111");
        var book2 = new Book(id, "Different Title", "Author Two", "2222222222");
        
        // Act
        storage.Add(book1);
        storage.Add(book2);
        
        // Assert
        var retrieved = storage.Get(id);
        Assert.NotNull(retrieved);
        // Since they have the same Id, they should be considered equal
        Assert.True(storage.Contains(book1));
        Assert.True(storage.Contains(book2)); // Should be true due to Id equality
    }
    
    [Fact]
    public void Add_DuplicateStudent_ShouldNotAddTwice()
    {
        // Arrange
        var storage = new Storage();
        var id = Guid.NewGuid();
        var student1 = new Student(id, new FullName("First", "Last"), "email1@test.com");
        var student2 = new Student(id, new FullName("Different", "Name"), "email2@test.com");
        
        // Act
        storage.Add(student1);
        storage.Add(student2);
        
        // Assert
        var retrieved = storage.Get(id);
        Assert.NotNull(retrieved);
        Assert.True(storage.Contains(student1));
        Assert.True(storage.Contains(student2));
    }
    
    #endregion
    
    #region Null and Empty Tests
    
    [Fact]
    public void Get_EmptyStorage_ShouldReturnNull()
    {
        // Arrange
        var storage = new Storage();
        
        // Act
        var result = storage.Get(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void Get_NonExistentGuid_ShouldReturnNull()
    {
        // Arrange
        var storage = new Storage();
        storage.Add(new Book(Guid.NewGuid(), "Existing", "Author", "1234567890"));
        
        // Act
        var result = storage.Get(Guid.NewGuid());
        
        // Assert
        Assert.Null(result);
    }
    
    #endregion
    
    #region Sequential Operations Tests
    
    [Fact]
    public void Storage_AddRemoveAdd_ShouldWorkCorrectly()
    {
        // Arrange
        var storage = new Storage();
        var employee = new Employee(
            Guid.NewGuid(),
            new FullName("Sequence", "Test"),
            "sequence.test@company.com",
            "Tester");
        
        // Act & Assert
        storage.Add(employee);
        Assert.True(storage.Contains(employee));
        
        storage.Remove(employee);
        Assert.False(storage.Contains(employee));
        
        storage.Add(employee);
        Assert.True(storage.Contains(employee));
    }
    
    [Fact]
    public void Storage_ComplexSequence_ShouldMaintainCorrectState()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book(Guid.NewGuid(), "Book1", "Author1", "1234567890");
        var student = new Student(Guid.NewGuid(), new FullName("Student", "One"), "s1@test.com");
        var employee = new Employee(Guid.NewGuid(), new FullName("Employee", "One"), "e1@test.com", "Role1");
        
        // Act
        storage.Add(book);
        storage.Add(student);
        Assert.True(storage.Contains(book));
        Assert.True(storage.Contains(student));
        
        storage.Remove(book);
        Assert.False(storage.Contains(book));
        Assert.True(storage.Contains(student));
        
        storage.Add(employee);
        Assert.False(storage.Contains(book));
        Assert.True(storage.Contains(student));
        Assert.True(storage.Contains(employee));
        
        storage.Remove(student);
        Assert.False(storage.Contains(book));
        Assert.False(storage.Contains(student));
        Assert.True(storage.Contains(employee));
    }
    
    #endregion
}