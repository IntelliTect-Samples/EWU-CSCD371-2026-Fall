using System.Text.Json;

namespace Contacts.Tests;

[TestClass]
public class ContactStorageTests
{
    private string _path = "";

    [TestInitialize]
    public void Initialize()
    {
        _path = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid()}.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        File.Delete(_path);
    }

    [TestMethod]
    public void SaveThenLoad_RoundTripsStudentsAndEmployees()
    {
        Student student = new(new FullName("Ada", "Lovelace", "King"), ["ada@example.com", "ada@ewu.edu"], "CS", "555-0100");
        Employee employee = new(new FullName("Grace", "Hopper"), ["grace@example.com"], "Admiral");
        ContactBook book = new();
        book.Add(student);
        book.Add(employee);

        ContactStorage.Save(book, _path);
        ContactBook loaded = ContactStorage.Load(_path);

        Assert.HasCount(2, loaded.GetAll());
        Assert.IsTrue(loaded.Contains(student));
        Assert.IsTrue(loaded.Contains(employee));

        Student loadedStudent = loaded.GetAll().OfType<Student>().Single();
        Employee loadedEmployee = loaded.GetAll().OfType<Employee>().Single();
        Assert.AreEqual(student.Id, loadedStudent.Id);
        Assert.AreEqual(employee.Id, loadedEmployee.Id);
        CollectionAssert.AreEqual(student.Emails.ToArray(), loadedStudent.Emails.ToArray());
        Assert.AreEqual("King", loadedStudent.Name.MiddleName);
        Assert.IsNull(loadedEmployee.Phone);
    }

    [TestMethod]
    public void Save_WritesTypeDiscriminator()
    {
        ContactBook book = new();
        book.Add(new Student(new FullName("Ada", "Lovelace"), ["ada@example.com"], "CS"));
        book.Add(new Employee(new FullName("Grace", "Hopper"), ["grace@example.com"], "Admiral"));

        ContactStorage.Save(book, _path);
        string json = File.ReadAllText(_path);

        StringAssert.Contains(json, "\"Type\": \"Student\"");
        StringAssert.Contains(json, "\"Type\": \"Employee\"");
    }

    [TestMethod]
    public void SaveThenLoad_EmptyBook_LoadsEmpty()
    {
        ContactStorage.Save(new ContactBook(), _path);

        Assert.IsEmpty(ContactStorage.Load(_path).GetAll());
    }

    [TestMethod]
    public void Save_NullBook_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ContactStorage.Save(null!, _path));
    }

    [TestMethod]
    public void Load_MissingFile_ReturnsEmptyBook()
    {
        Assert.IsEmpty(ContactStorage.Load(_path).GetAll());
    }

    [TestMethod]
    public void Load_JsonNull_ReturnsEmptyBook()
    {
        File.WriteAllText(_path, "null");

        Assert.IsEmpty(ContactStorage.Load(_path).GetAll());
    }

    [TestMethod]
    public void Load_NullEntries_AreSkipped()
    {
        File.WriteAllText(_path, "[null]");

        Assert.IsEmpty(ContactStorage.Load(_path).GetAll());
    }

    [TestMethod]
    public void Load_DuplicateEntries_AreStoredOnce()
    {
        ContactBook book = new();
        book.Add(new Student(new FullName("Ada", "Lovelace"), ["ada@example.com"], "CS"));
        ContactStorage.Save(book, _path);
        string entry = File.ReadAllText(_path).Trim().TrimStart('[').TrimEnd(']');
        File.WriteAllText(_path, $"[{entry},{entry}]");

        Assert.HasCount(1, ContactStorage.Load(_path).GetAll());
    }

    [TestMethod]
    public void Load_CorruptJson_Throws()
    {
        File.WriteAllText(_path, "{ not json");

        Assert.Throws<JsonException>(() => ContactStorage.Load(_path));
    }

    [TestMethod]
    public void Load_UnknownType_Throws()
    {
        File.WriteAllText(_path, """[{"Type":"Alien","Name":{"FirstName":"A","LastName":"B"}}]""");

        Assert.Throws<JsonException>(() => ContactStorage.Load(_path));
    }

    [TestMethod]
    public void Load_MissingType_Throws()
    {
        File.WriteAllText(_path, """[{"Name":{"FirstName":"A","LastName":"B"}}]""");

        Assert.Throws<NotSupportedException>(() => ContactStorage.Load(_path));
    }

    [TestMethod]
    public void Save_OverExistingFile_ReplacesItAndLeavesNoTempFile()
    {
        File.WriteAllText(_path, "old contents");
        ContactBook book = new();
        book.Add(new Student(new FullName("Ada", "Lovelace"), ["ada@example.com"], "CS"));

        ContactStorage.Save(book, _path);

        Assert.HasCount(1, ContactStorage.Load(_path).GetAll());
        Assert.IsFalse(File.Exists(_path + ".tmp"));
    }

    [TestMethod]
    public void Load_InvalidContactData_ThrowsFromValidation()
    {
        File.WriteAllText(_path, """[{"Type":"Student","Name":{"FirstName":"A","LastName":"B"},"Emails":["a@b.c"],"Major":" "}]""");

        Assert.Throws<ArgumentException>(() => ContactStorage.Load(_path));
    }
}
