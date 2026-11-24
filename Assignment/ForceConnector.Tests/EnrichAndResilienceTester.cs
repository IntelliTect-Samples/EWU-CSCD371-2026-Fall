using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;
using ForceConnector.Core.Services;
using Moq;

namespace ForceConnector.Tests;

[TestClass]
public class EnrichAndResilienceTester
{
    [TestMethod]
    public async Task EnrichAsyncPropagatesCallerCancellation()
    {
        Mock<IPeopleClient> peopleClient = new();
        Mock<ILocationClient> locationClient = new();

        peopleClient
            .Setup(client =>
                client.DownloadPeopleCsvAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                """
            FirstName,LastName,EmailAddress,ZipCode
            Mateo,Edwards,mateo@test.com,99201
            """);

        locationClient
            .Setup(client =>
                client.LookupAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new OperationCanceledException());

        PeopleEnricher enricher =
            new(
                peopleClient.Object,
                locationClient.Object,
                new CsvParser());

        using CancellationTokenSource cts = new();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => enricher.EnrichAsync(
                "Mateo",
                cts.Token));
    }
    [TestMethod]
    public async Task EnrichAsyncRejectsInvalidLocation()
    {
        Mock<IPeopleClient> peopleClient = new();
        Mock<ILocationClient> locationClient = new();

        peopleClient
            .Setup(client =>
                client.DownloadPeopleCsvAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                """
            FirstName,LastName,EmailAddress,ZipCode
            Mateo,Edwards,mateo@test.com,99201
            """);

        locationClient
            .Setup(client =>
                client.LookupAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Location
                {
                    City = "Spokane",
                    State = "Washington"
                });

        PeopleEnricher enricher =
            new(
                peopleClient.Object,
                locationClient.Object,
                new CsvParser());

        EnrichmentResult result =
            await enricher.EnrichAsync(
                "Mateo",
                CancellationToken.None);

        Assert.HasCount(0, result.People);
        Assert.HasCount(1, result.Rejections);

        StringAssert.Contains(
            result.Rejections[0].Reason,
            "two-letter abbreviation");
    }
    [TestMethod]
    public async Task EnrichAsyncReportsHttpFailure()
    {
        Mock<IPeopleClient> peopleClient = new();
        Mock<ILocationClient> locationClient = new();

        peopleClient
            .Setup(client =>
                client.DownloadPeopleCsvAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                """
            FirstName,LastName,EmailAddress,ZipCode
            Mateo,Edwards,mateo@test.com,99201
            """);

        locationClient
            .Setup(client =>
                client.LookupAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new HttpRequestException(
                    "Server exploded"));

        PeopleEnricher enricher =
            new(
                peopleClient.Object,
                locationClient.Object,
                new CsvParser());

        EnrichmentResult result =
            await enricher.EnrichAsync(
                "Mateo",
                CancellationToken.None);

        Assert.HasCount(1, result.Rejections);

        StringAssert.Contains(
            result.Rejections[0].Reason,
            "Server exploded");
    }
    [TestMethod]
    public async Task EnrichAsyncReportsSpecificValidationError()
    {
        Mock<IPeopleClient> peopleClient = new();
        Mock<ILocationClient> locationClient = new();

        peopleClient
            .Setup(client =>
                client.DownloadPeopleCsvAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                """
            FirstName,LastName,EmailAddress,ZipCode
            ,Edwards,mateo@test.com,99201
            """);

        PeopleEnricher enricher =
            new(
                peopleClient.Object,
                locationClient.Object,
                new CsvParser());

        EnrichmentResult result =
            await enricher.EnrichAsync(
                "Mateo",
                CancellationToken.None);

        Assert.HasCount(1, result.Rejections);

        StringAssert.Contains(
            result.Rejections[0].Reason,
            "First name is required");
    }
}
