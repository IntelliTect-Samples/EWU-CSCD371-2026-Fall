namespace Logger;

public class LogFactory
{
    public string? FileName { get; set; }
    //TODO: This Logger factor only supports FileLogger
    /// In future, we can extend it to support other logger types based on configuration
    public BaseLogger? CreateLogger(string className) => 
        FileName is null ? null : new FileLogger(className, FileName);

    public void ConfigureFileLogger(string fileName) => FileName=fileName;
}
