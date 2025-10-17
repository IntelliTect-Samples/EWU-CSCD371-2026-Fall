using System.Net;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CanHazFunny.Tests;

internal sealed class FakeHandler : HttpMessageHandler
{
	private readonly string _content;
	public FakeHandler(string content) => _content = content;
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
		Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(_content)
		});
}

internal sealed class CapturingHandler: HttpMessageHandler
{
	public Uri? LastUri { get; private set; }
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		LastUri = request.RequestUri;
		return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent("OK")
		});
	}
}

public class JokeServiceTests
{
	[Fact]
	public void GetJoke_ReturnsExpectedJoke()
	{
		// Arrange
		var expectedJoke = "Why do programmers prefer dark mode? Because light attracts bugs!";
		var fakeHandler = new FakeHandler(expectedJoke);
		var httpClient = new HttpClient(fakeHandler);
		var jokeService = new JokeService(httpClient, "http://fake-endpoint");
		// Act
		var joke = jokeService.GetJoke();
		// Assert
		Assert.Equal(expectedJoke, joke);
    }

	[Fact]
	public void GetJoke_UsesDefaultEndpoint_WhenEndpointNull()
	{
		// Arrange
		var fakeHandler = new CapturingHandler();
		var httpClient = new HttpClient(fakeHandler);
		var jokeService = new JokeService(httpClient, null);
		// Act
		_ = jokeService.GetJoke();
		// Assert
		Assert.Equal("https://geek-jokes.sameerkumar.website/api", fakeHandler.LastUri!.ToString());
    }
}
