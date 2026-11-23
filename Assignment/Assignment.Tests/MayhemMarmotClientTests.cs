using Assignment9.Generated;
using Microsoft.Kiota.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class MayhemMarmotClientTests(TestContext testContext)
{
    [TestMethod]
    public async Task GetPeopleAsync_RequestsPeopleForUser_WithoutChaosCode()
    {
        using var api = new FakeMayhemApi();
        using FakeConnection connection = api.Connect();

        PeopleParseResult result = await connection.Client.GetPeopleAsync("trever w", testContext.CancellationToken);

        Assert.HasCount(3, result.People);
        Uri uri = api.RequestUris.Single();
        Assert.AreEqual("/people/trever%20w", uri.AbsolutePath);
        Assert.AreEqual(string.Empty, uri.Query);
    }

    [TestMethod]
    public async Task GetLocationAsync_RequestsZip_AndParsesLocation()
    {
        using var api = new FakeMayhemApi();
        using FakeConnection connection = api.Connect();

        Location location = await connection.Client.GetLocationAsync("99201", testContext.CancellationToken);

        Assert.AreEqual(new Location("SPOKANE", "WA"), location);
        Uri uri = api.RequestUris.Single();
        Assert.AreEqual("/people/zip/99201", uri.AbsolutePath);
        Assert.AreEqual(string.Empty, uri.Query);
    }

    [TestMethod]
    public async Task GetLocationAsync_ServerError_ThrowsApiException()
    {
        using var api = new FakeMayhemApi { Zip = (_, _, _) => Responses.ServerError() };
        using FakeConnection connection = api.Connect();

        ApiException exception = await Assert.ThrowsAsync<ApiException>(
            () => connection.Client.GetLocationAsync("99201", testContext.CancellationToken));

        Assert.AreEqual(500, exception.ResponseStatusCode);
        Assert.IsTrue(RetryPolicy.IsTransient(exception));
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task GetLocationAsync_InvalidData_ThrowsInvalidResponseException_AndDisposesStream()
    {
        using var api = new FakeMayhemApi { Zip = (_, _, _) => Responses.InvalidData() };
        using FakeConnection connection = api.Connect();

        await Assert.ThrowsExactlyAsync<InvalidResponseException>(
            () => connection.Client.GetLocationAsync("99201", testContext.CancellationToken));

        Assert.AreEqual(1, api.BodyStreamCount);
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task GetPeopleAsync_InvalidData_ThrowsInvalidResponseException()
    {
        using var api = new FakeMayhemApi { People = (_, _) => Responses.InvalidData() };
        using FakeConnection connection = api.Connect();

        await Assert.ThrowsExactlyAsync<InvalidResponseException>(
            () => connection.Client.GetPeopleAsync(SampleData.User, testContext.CancellationToken));
    }

    [TestMethod]
    public async Task GetLocationAsync_Success_DisposesStream()
    {
        using var api = new FakeMayhemApi();
        using FakeConnection connection = api.Connect();

        await connection.Client.GetLocationAsync("99201", testContext.CancellationToken);

        Assert.AreEqual(1, api.BodyStreamCount);
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task GetLocationAsync_Hang_HonorsCancellation()
    {
        using var api = new FakeMayhemApi { Zip = (_, _, token) => Responses.Hang(token) };
        using FakeConnection connection = api.Connect();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => connection.Client.GetLocationAsync("99201", cancellation.Token));
    }

    [TestMethod]
    public void Constructor_NullApi_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new MayhemMarmotClient(null!));
    }

    [TestMethod]
    public void GeneratedClient_IsWhatTheWrapperUses()
    {
        // Guards the extra-credit requirement: the only constructor takes the generated client.
        Type[] parameterTypes = [.. typeof(MayhemMarmotClient).GetConstructors().Single().GetParameters().Select(p => p.ParameterType)];
        CollectionAssert.AreEqual(new[] { typeof(MayhemMarmotApiClient) }, parameterTypes);
    }
}
