using Logger.Entities.Books;
using Logger.Entities.People;
using Xunit;

namespace Logger.Tests;

public class StorageTests
{
    /// <summary>
    /// Common entity data used for parameterized tests across entity types.
    /// </summary>
    public static TheoryData<IEntity> EntityData =>
    [
        new Book("1984", new FullName("George", null, "Orwell")),
        new Employee(new FullName("Homer", null, "Simpson"), 52000),
        new Student(new FullName("Bart", null, "Simpson"), 0.1m)
    ];

    [Fact]
    public void Add_DifferentEntityTypesWithSameName_ReturnsFalseOnContains()
    {
        // Arrange
        Storage storage = new();
        Employee employee = new(new FullName("Marge", null, "Simpson"), 60000);
        Student student = new(new FullName("Marge", null, "Simpson"), 3);

        // Act
        storage.Add(employee);

        // Assert
        Assert.False(storage.Contains(student));
    }

    [Theory]
    [MemberData(nameof(EntityData))]
    public void Add_ThenContains_ReturnsTrue(IEntity entity)
    {
        // Arrange
        Storage storage = new();

        // Act
        storage.Add(entity);

        // Assert
        Assert.True(storage.Contains(entity));
    }

    [Theory]
    [MemberData(nameof(EntityData))]
    public void Get_ExistingEntity_ReturnsEntityWithSameIdAndName(IEntity entity)
    {
        // Arrange
        Storage storage = new();
        storage.Add(entity);

        // Act
        IEntity? retrieved = storage.Get(entity.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(entity.Id, retrieved!.Id);
        Assert.Equal(entity.Name, retrieved.Name);
    }

    [Fact]
    public void Get_UnknownId_ReturnsNull()
    {
        // Arrange
        Storage storage = new();

        // Act & Assert
        Assert.Null(storage.Get(Guid.NewGuid()));
    }

    [Theory]
    [MemberData(nameof(EntityData))]
    public void Remove_ExistingEntity_RemovesFromStorage(IEntity entity)
    {
        // Arrange
        Storage storage = new();

        // Act
        storage.Add(entity);
        storage.Remove(entity);

        // Assert
        Assert.Null(storage.Get(entity.Id));
    }

    [Theory]
    [MemberData(nameof(EntityData))]
    public void Remove_OneEntityAmongMany_RemovesOnlySpecifiedEntity(IEntity entityA)
    {
        // Arrange
        Storage storage = new();
        storage.Add(new Employee(new FullName("Waylon", null, "Smithers"), 58000));
        storage.Add(new Book("The Catcher in the Rye", new FullName("J.D.", null, "Salinger")));
        storage.Add(new Student(new FullName("Martin", null, "Prince"), 3.9m));

        // Pick another entity type to ensure it's distinct
        IEntity entityB = entityA switch
        {
            Book => new Student(new FullName("Lisa", null, "Simpson"), 4),
            Employee => new Book("Brave New World", new FullName("Aldous", null, "Huxley")),
            _ => new Employee(new FullName("Moe", null, "Szyslak"), 45000)
        };

        // Act
        storage.Add(entityA);
        storage.Add(entityB);
        storage.Remove(entityA);

        // Assert
        Assert.Null(storage.Get(entityA.Id));
        Assert.NotNull(storage.Get(entityB.Id));
    }

    [Theory]
    [MemberData(nameof(EntityData))]
    public void Remove_UnknownEntity_DoesNothing(IEntity entity)
    {
        // Arrange
        Storage storage = new();

        // Act
        storage.Remove(entity); // not added

        // Assert
        Assert.False(storage.Contains(entity));
    }
}
