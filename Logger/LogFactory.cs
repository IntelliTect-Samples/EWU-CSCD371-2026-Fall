using System.Runtime.CompilerServices;

namespace Logger;

public class LogFactory
{
    private string path;
    public void ConfigureFileLogger(string filePath)
    {
        path = filePath;

    }
    public BaseLogger CreateLogger(string className)
    {
        if (path == null) return null;

        return new FileLogger(path)
        {
            ClassName = className
        };


    }
}
