using Assignment;

namespace Assignment.Tests;

[TestClass]
public class PersonRecordParserTests
{
    [TestMethod]
    public void Parse_ValidLine_ReturnsPerson()
    {
        // Arrange
        string line = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577";
        // Act
        Person person = PersonRecordParser.Parse(line);
        // Assert
        Assert.IsNotNull(person);
        Assert.AreEqual(1, person.Id);
        Assert.AreEqual("Priscilla", person.FirstName);
        Assert.AreEqual("Jenyns", person.LastName);
        Assert.AreEqual("pjenyns0@state.gov", person.EmailAddress);
    }

    [TestMethod]
    [DataRow("1,Priscilla,Jenyns")]                                          // rule 1: too few fields
    [DataRow("abc,Priscilla,Jenyns,p@x.gov,7884 Corry Way,Helena,MT,70577")] // rule 2
    [DataRow("0,Priscilla,Jenyns,p@x.gov,7884 Corry Way,Helena,MT,70577")]   // rule 2
    [DataRow("1,,Jenyns,p@x.gov,7884 Corry Way,Helena,MT,70577")]            // rule 3: first name empty
    public void Parse_InvalidLine_ThrowsFormatException(string line)
    {
        // Act & Assert
        Assert.ThrowsExactly<FormatException>(() => PersonRecordParser.Parse(line));
    }
}
