namespace ForceConnector.Tests;

using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public sealed class ClientTesters
{
    [TestMethod]
    public async Task DownloadPeopleCsvAsyncThrowsForBlankUser()
    {
        HttpClient client = new();

        PeopleClient peopleClient =
            new(client);

        await Assert.ThrowsAsync<ArgumentException>(
            () => peopleClient.DownloadPeopleCsvAsync(
                string.Empty,
                CancellationToken.None));
    }
    [TestMethod]
    public async Task LocationLookupAsyncThrowsForBlankZip()
    {
        HttpClient client = new();

        LocationClient locationClient =
            new(client);

        await Assert.ThrowsAsync<ArgumentException>(
            () => locationClient.LookupAsync(
                string.Empty,
                CancellationToken.None));
    }
}
