using Assignment;

Console.WriteLine("Hello, World!");

var client = HttpClientWrapper.Client;

string response = await client.GetStringAsync("/people/zip/99252?code=None");



Console.WriteLine(response);

//Importer importer = new();

//var list = await importer.ImportPeopleAsync();

//foreach (var item in list)
//{
//    Console.WriteLine(item.ToString());
//}

//importer.PrintErrorsAndFlush();

