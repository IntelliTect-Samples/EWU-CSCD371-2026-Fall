using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class CsvRowsTests
{
    [TestMethod]
    public void CsvRows_FileDeployedAndHeaderSkipped_ReturnsCorrectLineCount()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "People.csv");
        Assert.IsTrue(File.Exists(path), "People.csv should be copied to output directory (Copy if newer).");
        string[] allLines = File.ReadAllLines(path);
        SampleData sample = new();
        string[] rows = sample.CsvRows.ToArray();
        Assert.HasCount(allLines.Length - 1, rows);
        string header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
        Assert.AreNotEqual(header, rows[0]);
    }

    [TestMethod]
    public void CsvRows_EnumerationCompletes_DisposesFileHandle()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "People.csv");
        Assert.IsTrue(File.Exists(path));
        SampleData sample = new();
        string[] rows = sample.CsvRows.ToArray();
        using FileStream stream = new(path, FileMode.Append, FileAccess.Write, FileShare.None);
        Assert.IsTrue(stream.CanWrite);
    }
}
