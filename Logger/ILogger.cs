namespace Logger
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string message);
        
        public static abstract ILogger Create(string className);
    }
}