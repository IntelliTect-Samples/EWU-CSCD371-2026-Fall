using Assignment;

namespace Tests;

public class DisposalTests
{
    [Fact]
    public void Import_Succeeds_DisposesReader()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice));
        using var importer = new PeopleCSVImporter(reader);

        importer.Import();

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Import_FailFastOnInvalidRecord_DisposesReader()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader("not,a,valid,record"));
        using var importer = new PeopleCSVImporter(reader);

        Assert.Throws<InvalidPersonRecordException>(() => importer.Import());

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Import_ReaderThrowsIOException_DisposesReader()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice), new IOException("Simulated."));
        using var importer = new PeopleCSVImporter(reader);

        Assert.Throws<IOException>(() => importer.Import());

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Import_CallbackThrows_DisposesReader()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader("0,Gus,King,gus@example.com,700 Oak St,Olympia,WA,98501"));
        using var importer = new PeopleCSVImporter(reader);

        Assert.Throws<InvalidOperationException>(
            () => importer.Import(_ => throw new InvalidOperationException("Callback failed.")));

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Dispose_WithoutImport_DisposesReader()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice));
        var importer = new PeopleCSVImporter(reader);

        importer.Dispose();

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Dispose_CalledRepeatedly_DisposesReaderOnce()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice));
        var importer = new PeopleCSVImporter(reader);

        importer.Dispose();
        importer.Dispose();
        importer.Dispose();

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Dispose_AfterImport_DoesNotDisposeReaderAgain()
    {
        var reader = new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice));
        using (var importer = new PeopleCSVImporter(reader))
        {
            importer.Import();
        }

        Assert.Equal(1, reader.DisposeCount);
    }

    [Fact]
    public void Import_AfterDispose_ThrowsObjectDisposedException()
    {
        var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice)));
        importer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => importer.Import());
        Assert.Throws<ObjectDisposedException>(() => importer.Import(_ => { }));
    }

    [Fact]
    public void Import_CalledTwice_ThrowsObjectDisposedException()
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice)));
        importer.Import();

        Assert.Throws<ObjectDisposedException>(() => importer.Import());
    }

    [Fact]
    public void Import_FromTemporaryFile_ReleasesFileSoItCanBeDeleted()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, TestCsv.WithHeader(TestCsv.Alice));
            using var importer = new PeopleCSVImporter(path);

            importer.Import();

            // On Windows, deleting a file that is still open throws IOException.
            File.Delete(path);
            Assert.False(File.Exists(path));
        }
        finally
        {
            // Clean up even if the import or an assertion above failed.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
