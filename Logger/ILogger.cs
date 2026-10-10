namespace Logger;

public interface ILogger<T> where T : BaseLogger
{
    /// <summary>
    /// Factory method for creating a new logger with type T
    /// </summary>
    /// <param name="className"></param> The name of the class
    /// <returns></returns> New instance of ILogger
    static abstract T CreateLogger(string className);
}

