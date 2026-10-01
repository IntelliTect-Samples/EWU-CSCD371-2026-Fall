namespace Logger;

// an example caller using what I believe to be everything that meets the base assignment
public class ExampleCaller
{
    // a single logger allocation for this class to use
    private readonly BaseLogger _logger;

    public ExampleCaller()
    {
        // 3 lines of boilerplate per class in order to perform logging operations
        // this is allocations PER class to create a factory instance and associated logger
        var factory = new LogFactory();
        factory.ConfigureFileLogger("filePath.txt");
        _logger = factory.CreateLogger(nameof(ExampleCaller));
            
        // bypass factory is allowed? (at least this now does one allocation and 'less' boilerplate?)
        // this will not 'set' the ClassName property however
        _logger = new FileLogger("filePath.txt");
    }
        
    public void Foo()
    {
        // NRE possibility (nothing guarantees you remembered to call config prior to creation of logger)
        // therefore usage of null propagation is required for every call
        _logger?.Log(LogLevel.Information, "Foo");
    }
}
    
// PROS: ***************************************************************
    
// each class can specify the file in which it will print logs related to it
    
// CONS: ***************************************************************
    
// logs can potentially hard to read as you must find the correct file associated with the specific class
// and each class can configure to a unique location
    
// every class allocates a creation of a factory to make its log - defeats purpose of factory? *see notes below*
    
// nothing prevents bypassing the factory?
// _logger = new FileLogger("filePath.txt"); still works
    
// every class has boilerplate for setting up the logger in the first place
    
// no linkage between config and logger creation (could read NRE because you did not config prior to creation)
    
// NOTES: ***************************************************************
    
// even if you did a single factory instance, you'd want to link the config with the creation.
// to ensure the BaseLogger instance retrieved will log to the location you desire.
// else you could be logging to wherever the last configure set the file too.

// EXCEPTION: *************************

// if this ExampleCaller class was actually your static instance in which to call logging operations from.
// This class creates the only factory needed (even though it could still bypass the factory)
// and exposes operations like Log, LogError, LogWarning etc. with which called use to log some information.
// only issue is now the 'ClassName' property now becomes meaningless.