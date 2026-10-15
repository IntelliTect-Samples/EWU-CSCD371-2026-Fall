namespace CanHazFunny;

using System;

/// <summary>
/// Represents a service that outputs jokes to the console.
/// </summary>
public interface IJokeConsoleOutput
{
    void WriteLine(string joke);
}