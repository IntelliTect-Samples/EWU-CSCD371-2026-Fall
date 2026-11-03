using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class LogHighInTheCallStack
{
    // Think of Run as the UI layer, GreetCustomer as the business layer,
    // and FindCustomerName as the data-access layer.
    public static void Run()
    {
        Console.WriteLine("BEFORE: every layer logs and rethrows");
        Log.Reset();
        try
        {
            Console.WriteLine(GreetCustomerNoisy(42));
        }
        catch (KeyNotFoundException exception)
        {
            Log.Error(nameof(Run), exception);
        }
        Console.WriteLine($"  Log entries for ONE failure: {Log.Count}");

        Console.WriteLine();
        Console.WriteLine("AFTER: only the top of the call stack logs");
        Log.Reset();
        try
        {
            Console.WriteLine(GreetCustomer(42));
        }
        catch (KeyNotFoundException exception)
        {
            Log.Error(nameof(Run), exception);
        }
        Console.WriteLine($"  Log entries for ONE failure: {Log.Count}");
    }

    private static string FindCustomerName(int id) => id switch
    {
        1 => "Inigo Montoya",
        2 => "Princess Buttercup",
        _ => throw new KeyNotFoundException($"Customer {id} was not found."),
    };

    // ----- BEFORE -----
    private static string FindCustomerNameNoisy(int id)
    {
        try
        {
            return FindCustomerName(id);
        }
        catch (Exception exception)
        {
            Log.Error(nameof(FindCustomerNameNoisy), exception);
            throw;
        }
    }

    private static string GreetCustomerNoisy(int id)
    {
        try
        {
            return $"Hello, {FindCustomerNameNoisy(id)}!";
        }
        catch (Exception exception)
        {
            Log.Error(nameof(GreetCustomerNoisy), exception);
            // Where does this go in a web API? A Windows service? Nowhere anyone will see it.
            Console.WriteLine("  Oops! Something went wrong.");
            throw;
        }
    }

    // ----- AFTER -----
    private static string GreetCustomer(int id) => $"Hello, {FindCustomerName(id)}!";
}
