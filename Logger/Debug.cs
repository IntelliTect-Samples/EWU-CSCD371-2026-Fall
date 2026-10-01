namespace Logger;

public static class Debug
{
    private static BaseLogger s_logger;

    public static void Configure(string filePath)
    {
        var factory = new LogFactory();
        factory.ConfigureFileLogger(filePath);
        s_logger = factory.CreateLogger(nameof(Debug));
    }

    public static void Log(string message)
    {
        s_logger?.Information(message);
    }
}
    
// what becomes the point of setting the "ClassName" property here?
// it will always be this Debug class (if you follow the correct factory setup)