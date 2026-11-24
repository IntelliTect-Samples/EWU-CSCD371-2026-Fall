using ForceConnector.Core.Services;
using ForceConnector.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ForceConnector.Tests;

[TestClass]
public class CsvWriterTester
{
    [TestMethod]
    public async Task WriteAsyncCreatesCsv()
    {
        string path = Path.GetTempFileName();

        try
        {
            await CsvWriter.WriteAsync(
                path,
                [],
                CancellationToken.None);

            Assert.IsTrue(File.Exists(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public async Task WriteAsyncEscapesSpecialFields()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"people-{Guid.NewGuid():N}.csv");

        try
        {
            await CsvWriter.WriteAsync(
                path,
                [new EnrichedPerson
                {
                    FirstName = "Mateo, Jr.",
                    LastName = "Edwards",
                    EmailAddress = "mateo@example.com",
                    ZipCode = "99201",
                    City = "Spokane",
                    State = "WA"
                }],
                CancellationToken.None);

            string[] lines = await File.ReadAllLinesAsync(path);

            Assert.AreEqual(
                "\"Mateo, Jr.\",Edwards,mateo@example.com,99201,Spokane,WA",
                lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
