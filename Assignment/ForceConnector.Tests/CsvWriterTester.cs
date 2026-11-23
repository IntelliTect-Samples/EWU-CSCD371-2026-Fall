using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ForceConnector.Tests;

[TestClass]
public class CsvWriterTester
{
    [TestMethod]
    public async Task WriteAsync_CreatesCsv()
    {
        CsvWriter writer = new();

        string path =
            Path.GetTempFileName();

        try
        {
            await writer.WriteAsync(
                path,
                [],
                CancellationToken.None);

            Assert.IsTrue(
                File.Exists(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
