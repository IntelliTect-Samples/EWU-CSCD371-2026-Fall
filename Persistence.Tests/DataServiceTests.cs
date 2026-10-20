using Contacts;

namespace Persistence.Tests;

[TestClass]
public class DataServiceTests
{
    private static readonly FullName TestName = new("John", "Smith");
    private static readonly ContactEmails TestContactEmails = new(["test@email"]);

    private static Student CreateStudent() => new(TestName, TestContactEmails)
    {
        Id = new ContactId(Student.Prefix, 1)
    };
    private static Employee CreateEmployee() => new(TestName, TestContactEmails)
    {
        Id = new ContactId(Employee.Prefix, 1)
    };
    
    [TestMethod]
    public void SaveThenLoad_ReturnsSavedContacts()
    {
        // arrange
        FakeDataService service = new();

        IContact[] createdList =
        [
            CreateStudent(),
            CreateEmployee()
        ];
        
        // act
        service.Save(createdList);
        IContact[] savedList = service.Load();
        
        // assert
        CollectionAssert.AreEqual(createdList, savedList);
    }
    
    [TestMethod]
    public void Save_WithExistingList_ReplacesExistingContacts()
    {
        // arrange
        FakeDataService service = new();
        
        IContact[] originalList =
        [
            CreateStudent()
        ];
        
        service.Save(originalList);
        
        // act
        IContact[] overwriteList =
        [
            CreateStudent(),
            CreateEmployee()
        ];
        
        service.Save(overwriteList);
        IContact[] savedList = service.Load();
        
        // assert
        CollectionAssert.AreNotEqual(originalList, savedList);
        CollectionAssert.AreEqual(overwriteList, savedList);
    }

    [TestMethod]
    public void SaveThenLoad_PreservesContactId()
    {
        FakeDataService service = new();
        
        Student student = CreateStudent();
        
        service.Save([student]);
        
        Student loaded = (Student)service.Load()[0];
        
        Assert.AreEqual(student.Id, loaded.Id);
    }
}

public class FakeDataService : IDataService
{
    private IContact[] _contacts = [];

    public void Save(IEnumerable<IContact> contacts)
    {
        _contacts = contacts.ToArray();
    }
    
    public IContact[] Load() => _contacts;
}