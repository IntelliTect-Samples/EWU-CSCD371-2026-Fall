using System;
// using System.Collections.Generic; // No longer needed

namespace CanHazFunny;

public class ConsoleOutput : IOutput
{
    // FIX: Changed type to string? to store the last written message for testing.
    public string? Output { get; set; }

    public void WriteLine(string message)
    {
        // Store the message for unit testing
        Output = message;
        
        Console.WriteLine(message);
    }
}