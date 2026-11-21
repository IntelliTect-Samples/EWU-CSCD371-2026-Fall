using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class RejectionReportBuilderTests
{
    [TestMethod]
    public void BuildGroupsMatchingRejectionReasons()
    {
        RejectedPerson[] rejections =
        [
            new(RejectionStage.PeopleCsv, 2, "Row 2", "Invalid email."),
            new(RejectionStage.PeopleCsv, 3, "Row 3", "Invalid email."),
            new(RejectionStage.ZipLookup, null, "Jane Doe", "BAD_DATA.")
        ];

        EnrichmentResult result = new([], rejections);

        RejectionReport report = RejectionReportBuilder.Build(result);

        Assert.AreEqual(3, report.TotalRejected);
        Assert.HasCount(2, report.ReasonSummaries);

        Assert.AreEqual(RejectionStage.PeopleCsv, report.ReasonSummaries[0].Stage);
        Assert.AreEqual("Invalid email.", report.ReasonSummaries[0].Reason);
        Assert.AreEqual(2, report.ReasonSummaries[0].Count);

        Assert.AreEqual(RejectionStage.ZipLookup, report.ReasonSummaries[1].Stage);
        Assert.AreEqual("BAD_DATA.", report.ReasonSummaries[1].Reason);
        Assert.AreEqual(1, report.ReasonSummaries[1].Count);
    }

    [TestMethod]
    public void BuildKeepsSameReasonFromDifferentStagesSeparate()
    {
        RejectedPerson[] rejections =
        [
            new(RejectionStage.PeopleCsv, 2, "Row 2", "Invalid data."),
            new(RejectionStage.ZipLookup, null, "Jane Doe", "Invalid data.")
        ];

        RejectionReport report =
            RejectionReportBuilder.Build(new([], rejections));

        Assert.HasCount(2, report.ReasonSummaries);
        Assert.AreEqual(1, report.ReasonSummaries[0].Count);
        Assert.AreEqual(1, report.ReasonSummaries[1].Count);
    }

    [TestMethod]
    public void BuildPreservesIndividualRejectionDetails()
    {
        RejectedPerson rejection = new(
            RejectionStage.PeopleCsv,
            7,
            "Jane,Doe,invalid-email,99201",
            "EmailAddress is invalid.");

        RejectionReport report = RejectionReportBuilder.Build(
            new([], [rejection]));

        RejectedPerson actual = report.Details.Single();

        Assert.AreSame(rejection, actual);
        Assert.AreEqual(7, actual.LineNumber);
        Assert.AreEqual(
            "Jane,Doe,invalid-email,99201",
            actual.Subject);
    }

    [TestMethod]
    public void BuildEmptyResultReturnsEmptyReport()
    {
        RejectionReport report =
            RejectionReportBuilder.Build(new([], []));

        Assert.AreEqual(0, report.TotalRejected);
        Assert.IsEmpty(report.ReasonSummaries);
        Assert.IsEmpty(report.Details);
    }

    [TestMethod]
    public void BuildNullResultThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => RejectionReportBuilder.Build(null!));
    }
}