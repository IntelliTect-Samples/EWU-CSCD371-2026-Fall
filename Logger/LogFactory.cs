namespace Logger;

public class LogFactory
{

    private string? _filePath;

    public void ConfigureFileLogger(string filePath)
    {
        _filePath = filePath;
    }
    public BaseLogger? CreateLogger(string className)
    {
        if (_filePath is null)
        {
            return null;
        }

        return new FileLogger(_filePath) {ClassName = className};
    }
}
