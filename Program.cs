// Main is here
using System.Diagnostics;

Stopwatch stopwatch = Stopwatch.StartNew();
int i = 0;

WriteData();
return;

CancellationTokenSource cts = new();
//cts.CancelAfter(150);
try
{
    var temp = await MyWeather(cts.Token);
    Console.WriteLine($"The average temperature is {temp}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was canceled.");
}

//MyWeather();
Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms"); //<-- get down to about 200ms-ish
Console.WriteLine($"i = {i}");


async Task<double> MyWeather(CancellationToken token)
{
    token.ThrowIfCancellationRequested();

    i++;
    //Run in the background, really slow method
    Task<string> zipTask = Task.Run(() => GetZip());
    string zip = await zipTask;

    i++;
    
    Task<double> t1 = GetWeather(zip, "Provider1", token);
    //Pretend provider 2 is down
    Task<double> t2 = GetWeather(zip, "Provider2", token);
    Task<double> t3 = GetWeather(zip, "Provider3", token);

    /*
    Task<double> firstTask = await Task.WhenAny(t1, t2, t3);
    double firstTemp = await firstTask;
    return firstTemp;
    */

    

    try
    {
        Task.WaitAll(t1, t2, t3);
        return await t1 + await t2 + await t3;
        //double[] results = await Task.WhenAll(t1, t2, t3);
        //double avg = results.Average();
        //return avg;
    }
    catch(AggregateException ex)
    {
        return 12;
    }
    
} //.Result, .Wait(), .GetAwaiter(), .GetResult(), Thread class, bad bad bad

async Task WriteData()
{
    await Task.CompletedTask;
    // Does this run async?
    // If called without an await does this run to completion?
    for(int i = 0; i < 10; i++)
    {
        Console.WriteLine($"Writing data {i}");
    }

    // Do nothing, return a task
    //return Task.FromException(new InvalidOperationException("Go boom"));
    // return Task.CompletedTask;
}

string GetZip()
{
    return "99208";
}

// Can this method implement IWeatherProvider?
async Task<double> GetWeather(string zip, string provider, CancellationToken token)
{
    int delay = Random.Shared.Next(100, 200); //in ms
    await Task.Delay(delay, token);

    /*
    if (!provider.EndsWith("2", StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"Provider {provider} is down");
    }
    */
    return 42.0;
}

public interface IWeatherProvider
{
    Task<double> GetWeather(string zip, CancellationToken token);
}