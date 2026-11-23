using Assignment;

//Setup the HttpClientFactory
var services = new ServiceCollection();

services.AddHttpClient(HttpClientWrapper.ClientName, client =>
{
    client.BaseAddress = new Uri("https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io");
    //client.Timeout = TimeSpan.FromSeconds(60); //Todo: set timeout if needed
    client.DefaultRequestHeaders.Add("Accept", "text/csv");
});

services.AddTransient<HttpClientWrapper>();

using ServiceProvider provider = services.BuildServiceProvider();
//Get the HttpClientWrapper from the service provider
var httpClientWrapper = provider.GetRequiredService<HttpClientWrapper>(); 

//CancellationToken token = CancellationToken.None; //todo: implement cancelation token if needed
using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
CancellationToken token = cts.Token;
//hard-coding the user 
var user = "JohnM";

//Call the DownloadCsvAsync method
//Build the report: download people, look up each ZIP, combine
var (status, rows) = await PeopleZipReport.BuildAsync(httpClientWrapper, user, token);

//utilize the result
if (status == DownloadStatus.Success)
{
    Console.WriteLine("Download successful!");
    foreach (string row in rows)
    {
        Console.WriteLine(row);
    }
}
else
{
    Console.WriteLine($"Download failed with status: {status}");
}

