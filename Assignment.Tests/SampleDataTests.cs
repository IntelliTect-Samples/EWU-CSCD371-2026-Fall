using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_loadsPeopleCvs_notNull()
    {
        //Assign
        SampleData sampleData = new SampleData();
        List<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.IsNotNull(csvOut);
    }
    [TestMethod]
    public void CsvRows_loadsPeopleCvs_ignoresFirstRow()
    {
        //Assign
        SampleData sampleData = new SampleData();
        List<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.HasCount(50, csvOut);//CSV is of size 51, we should be getting 50.
        Assert.AreNotEqual("Id,FirstName,LastName,Email,StreetAddress,City,State,Zip", csvOut[0]);
    }
}
