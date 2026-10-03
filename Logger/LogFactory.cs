namespace Logger;

public class LogFactory
{

    private string filePath;

    public void ConfigureFileLogger(string fp)
    {
        filePath = fp;
    }

    public BaseLogger CreateLogger(string className)
    {
        if (filePath is null)
        {
            return null;
        }
        return new FileLogger(filePath) {className = className};
    }
}
