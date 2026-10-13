using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

/// <summary>
/// Unit tests for the Program class. These tests verify that the Program correctly retrieves jokes from the JokeService and passes them to the OutputService. The tests use mock implementations of IJokeService and IOutputService to ensure that the Program behaves as expected without relying on external services or console output.
/// </summary>
public class ProgramOutputTests
{

    /// <summary>
    /// Tests that the Run method retrieves a joke from the IJokeService and passes it to the IOutputService. The test uses mock implementations of IJokeService and IOutputService to verify that the Program correctly interacts with these services, ensuring that the joke is retrieved and output as expected.
    /// </summary>
    [Fact]
    public void RunRetrievesJokeAndPassesItToOutputService()
    {
        const string joke = "A deterministic joke.";
        Mock<IJokeService> jokeService = new();
        Mock<IOutputService> outputService = new();
        jokeService.Setup(service => service.GetJoke()).Returns(joke);

        Program.Run(jokeService.Object, outputService.Object);

        jokeService.Verify(service => service.GetJoke(), Times.Once);
        outputService.Verify(service => service.Output(joke), Times.Once);
    }

    /// <summary>
    /// Tests that the Main method invokes the Run method with real implementations of IJokeService and IOutputService, and that the joke retrieved from the JokeService is written to the console output. The test uses a stubbed HttpMessageHandler to simulate the JokeService response, allowing for deterministic testing without relying on external services or console output.
    /// </summary>
    [Fact]
    public void MainInvokeUsesRealServicesAndWritesTheRetrievedJoke()
    {
        const string joke = "A deterministic joke from the actual JokeService.";
        using HttpClient httpClient = new(new JokeResponseHandler(joke));
        ConsoleOutputService outputService = new();
        int jokeServiceFactoryCalls = 0;
        int outputServiceFactoryCalls = 0;
        TextWriter originalOutput = Console.Out;
        using StringWriter output = new();

        try
        {
            Console.SetOut(output);
            Program.Run(
                () =>
                {
                    jokeServiceFactoryCalls++;
                    return new JokeService(httpClient);
                },
                () =>
                {
                    outputServiceFactoryCalls++;
                    return outputService;
                });
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        Assert.Equal(1, jokeServiceFactoryCalls);
        Assert.Equal(1, outputServiceFactoryCalls);
        Assert.Equal(joke + Environment.NewLine, output.ToString());
    }

    /// <summary>
    /// Tests that the ConsoleOutputService correctly writes a message to the console output. The test temporarily redirects the console output to a StringWriter, invokes the Output method of the ConsoleOutputService, and verifies that the expected message is written to the console output.
    /// </summary>
    [Fact]
    public void ConsoleOutputServiceOutputWritesMessageToConsole()
    {
        const string message = "Test message";
        TextWriter originalOutput = Console.Out;
        using StringWriter output = new();

        try
        {
            Console.SetOut(output);
            new ConsoleOutputService().Output(message);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        Assert.Equal(message + Environment.NewLine, output.ToString());
    }

    /// <summary>
    /// A stubbed HttpMessageHandler that simulates a successful response from the JokeService API. This class is used in unit tests to provide a deterministic joke response without making actual HTTP requests, enabling testing of the Program class and its interaction with the JokeService.
    /// </summary>
    /// <param name="joke"></param>
    private sealed class JokeResponseHandler(string joke) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Assert.Equal("https://geek-jokes.sameerkumar.website/api", request.RequestUri?.ToString());

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(joke)
            });
        }
    }
}






// Notes: 
// * Current test fails to cover lines 12-14 in Program.cs
// * Might be an issue with perms of the function