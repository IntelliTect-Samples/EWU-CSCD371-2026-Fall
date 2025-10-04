using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Reflection;

namespace Logger;

public class LogFactory
{
    private string? FilePath { get; set; }
    public void ConfigureFileLogger(string loggerFilePath)
    {
        FilePath = loggerFilePath;
    }

    public T? CreateLogger<T>(string className)
    where T : BaseLogger
    {
        MethodInfo method = typeof(T).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);

        return (T?)method?.Invoke(null, [className, FilePath]);
    }
}
