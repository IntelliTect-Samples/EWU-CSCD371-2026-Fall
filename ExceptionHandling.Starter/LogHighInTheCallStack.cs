using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

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

        // TODO: Write GreetCustomer(id) that calls FindCustomerName directly with no try/catch,
        //       then copy the block above to call it and compare the log count.
    }

    private static string FindCustomerName(int id) => id switch
    {
        1 => "Inigo Montoya",
        2 => "Princess Buttercup",
        _ => throw new KeyNotFoundException($"Customer {id} was not found."),
    };

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
            Console.WriteLine("  Oops! Something went wrong.");
            throw;
        }
    }
}
