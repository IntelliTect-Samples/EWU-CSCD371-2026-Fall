using System;
using System.Net.Http;

using CanHazFunny.Core;
using CanHazFunny.Output;
using CanHazFunny.Services;

namespace CanHazFunny.CanHazFunny.Core;

class Program
{
    static async void Main(string[] args)
    {
        Console.WriteLine("Hello, CanHazFunny!");
        //Feel free to use your own setup here - this is just provided as an example
        //new Jester(new SomeReallyCoolOutputClass(), new SomeJokeServiceClass()).TellJoke();
        

        HttpClient http = new HttpClient();
        var jokerService = new JokeService(http);
        var output = new ConsoleJokingOutput();

        var jester = new Jester(jokerService, output);
        await jester.TellJoke();
    }
}
