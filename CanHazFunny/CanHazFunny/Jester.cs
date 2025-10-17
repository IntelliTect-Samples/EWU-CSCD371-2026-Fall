using System;
namespace CanHazFunny;

public class Jester
{

	private readonly IOutput _output;
	private readonly IJokeService _jokeService;
	private const int MaxAttempts = 10;

	public Jester(IOutput output, IJokeService jokeService)
	{
		_output = output ?? throw new ArgumentNullException(nameof(output));
		_jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));

	}

	public void TellJoke()
	{
		for (int i = 0; i < MaxAttempts; i++)
		{
			var joke = _jokeService.GetJoke();
			if (!joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase))
			{
				_output.WriteLine(joke);
				return;
			}
			
		}
		_output.WriteLine("Failed to find an acceptable joke. Try again!");
	}
}
