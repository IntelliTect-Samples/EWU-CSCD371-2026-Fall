

using Assignment;

//Console.WriteLine("Hello, World!");

//var client = HttpClientWrapper.Client;

//string response = await client.GetStringAsync("/people/Ben");

//Console.WriteLine(response);

Importer importer = new();

var list = await importer.ImportPeopleAsync();

foreach (var item in list)
{
    Console.WriteLine(item.ToString());
}

importer.PrintErrorsAndFlush();