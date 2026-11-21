

using Assignment;

Console.WriteLine("Hello, World!");

var client = HttpClientWrapper.Client;

string response = await client.GetStringAsync("/people/Ben");

Console.WriteLine(response);