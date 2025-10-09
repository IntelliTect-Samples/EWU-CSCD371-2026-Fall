using System;
using System.Reflection;

namespace Logger;

public class LogFactory
{
    private string? _FilePath { get; set; }
    public void ConfigureFileLogger(string loggerFilePath)
    {
        _FilePath = loggerFilePath;
    }

    public T? CreateLogger<T>(string className)
    where T : BaseLogger
    {
        MethodInfo? method = typeof(T).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
        if (method == null)
            throw new InvalidOperationException($"{typeof(T).Name} must have a public static Create method.");

        return (T?)method.Invoke(null, [className, _FilePath]);
    }
}
