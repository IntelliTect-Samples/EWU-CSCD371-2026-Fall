namespace Logger;

public interface IBaseLogger
{

    //Auto prop. for class name
    string ClassName { get; set; }


    void Log(LogLevel logLevel, string message);

}