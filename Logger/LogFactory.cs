namespace Logger;

public class LogFactory
{
    private string? filePath;
    public void ConfigureFileLogger(string path)
    {
        filePath = path;
    }

    public BaseLogger? CreateLogger(string className)
    {

        if (filePath == null)
            return null;

        return new FileLogger(filePath) { ClassName = className };

    }
}
