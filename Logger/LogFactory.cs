namespace Logger;


public class LogFactory
{
    private string? filePath;

    // req3:
    // [x] The LogFactory should be updated with a new method ConfigureFileLogger.
    // [x] This should take in a file path
    // [x] and store it in a private member.
    // [x] It should use this when instantiating a new FileLogger in its CreateLogger method
    public void ConfigureFileLogger(string path)
    {
        filePath = path;
    }

    public BaseLogger? CreateLogger(string className)
    {
        _filePath = filePath;
    }

        if (filePath == null)
            return null;

        return new FileLogger(filePath) { ClassName = className };

    }
}
