using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class CsvFileWriterTests(TestContext testContext)
{
    [TestMethod]
    public async Task WriteAtomicallyAsync_WritesAllLines_AndLeavesNoTempFile()
    {
        using var temp = new TempDirectory();
        string path = temp.PathOf("out.csv");

        await CsvFileWriter.WriteAtomicallyAsync(path, ["a,b", "c,d"], testContext.CancellationToken);

        CollectionAssert.AreEqual(new[] { "a,b", "c,d" }, await File.ReadAllLinesAsync(path, testContext.CancellationToken));
        CollectionAssert.AreEqual(new[] { path }, temp.Files());
    }

    [TestMethod]
    public async Task WriteAtomicallyAsync_ExistingFile_IsReplaced()
    {
        using var temp = new TempDirectory();
        string path = temp.PathOf("out.csv");
        await File.WriteAllTextAsync(path, "old", testContext.CancellationToken);

        await CsvFileWriter.WriteAtomicallyAsync(path, ["new"], testContext.CancellationToken);

        CollectionAssert.AreEqual(new[] { "new" }, await File.ReadAllLinesAsync(path, testContext.CancellationToken));
    }

    [TestMethod]
    public async Task WriteAtomicallyAsync_FailureWhileWriting_DeletesTempFile_AndKeepsExistingFile()
    {
        using var temp = new TempDirectory();
        string path = temp.PathOf("out.csv");
        await File.WriteAllTextAsync(path, "old", testContext.CancellationToken);

        await Assert.ThrowsExactlyAsync<IOException>(
            () => CsvFileWriter.WriteAtomicallyAsync(path, LinesThenFailure(), testContext.CancellationToken));

        CollectionAssert.AreEqual(new[] { path }, temp.Files());
        Assert.AreEqual("old", await File.ReadAllTextAsync(path, testContext.CancellationToken));
    }

    [TestMethod]
    public async Task WriteAtomicallyAsync_Cancelled_DeletesTempFile_AndWritesNothing()
    {
        using var temp = new TempDirectory();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => CsvFileWriter.WriteAtomicallyAsync(temp.PathOf("out.csv"), ["a"], cancellation.Token));

        Assert.IsEmpty(temp.Files());
    }

    [TestMethod]
    public async Task WriteAtomicallyAsync_CancelledPartWayThrough_DeletesTempFile()
    {
        using var temp = new TempDirectory();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => CsvFileWriter.WriteAtomicallyAsync(temp.PathOf("out.csv"), CancelAfterFirstLine(cancellation), cancellation.Token));

        Assert.IsEmpty(temp.Files());
    }

    private static IEnumerable<string> LinesThenFailure()
    {
        yield return "first";
        throw new IOException("Simulated disk failure.");
    }

    private static IEnumerable<string> CancelAfterFirstLine(CancellationTokenSource cancellation)
    {
        yield return "first";
        cancellation.Cancel();
        yield return "second";
    }
}
