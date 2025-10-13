using Xunit;
using Moq;
using System;
using System.IO;
using System.Net.Http;

namespace CanHazFunny.Tests;

// ----------------------------------------------------------------------
// MOCKABLE CLASSES (Used for fast Unit Tests)
// ----------------------------------------------------------------------

// Helper class to override JokeService's network call behavior for fast, targeted testing.
public class MockableJokeService : JokeService
{
    private readonly bool _shouldThrow;
    private readonly string? _nextJoke; // Made nullable for null test
    private int _callCount; // Fix for CA1805

    public MockableJokeService(string? nextJoke = "Not a Chuck Norris joke", bool shouldThrow = false)
    {
        _nextJoke = nextJoke;
        _shouldThrow = shouldThrow;
    }

    // This method now overrides the virtual method in JokeService.
    protected override string? GetJokeFromApi() 
    {
        if (_shouldThrow)
        {
            throw new HttpRequestException("Simulated API error");
        }
        
        // This is primarily for the Jester test's SetupSequence
        if (_nextJoke != null && _nextJoke.Contains("Chuck Norris") && _callCount == 0)
        {
            _callCount++;
            return _nextJoke;
        }
        
        // This returns the next joke string OR null (for the new test case)
        return _nextJoke;
    }
}

// ----------------------------------------------------------------------
// JESTER TESTS (100% LINE & BRANCH COVERAGE)
// ----------------------------------------------------------------------

public class JesterTests
{
    [Fact]
    public void GetJoke_ReturnsJokeFromJokeService()
    {
        // Arrange
        var mockJokeService = new Mock<IJokeService>();
        var mockOutputService = new Mock<IOutput>();
        var expectedJoke = "Why did the programmer quit? He didn't get arrays!";

        mockJokeService.Setup(js => js.GetJoke()).Returns(expectedJoke);
        var jester = new Jester(mockOutputService.Object, mockJokeService.Object);


        // Act
        var result = jester.GetJoke(); 


        // Assert
        Assert.Equal(expectedJoke, result);
        mockJokeService.Verify(js => js.GetJoke(), Times.Once);
    }

    [Fact]
    public void TellJoke_CallsOutputServiceWithJoke()
    {
        // Arrange
        var mockJokeService = new Mock<IJokeService>();
        var mockOutputService = new Mock<IOutput>();
        var testJoke = "Test joke";

        mockJokeService.Setup(js => js.GetJoke()).Returns(testJoke);
        var jester = new Jester(mockOutputService.Object, mockJokeService.Object);


        // Act
        jester.TellJoke();


        // Assert
        mockOutputService.Verify(os => os.WriteLine(testJoke), Times.Once); 
        mockJokeService.Verify(js => js.GetJoke(), Times.Once);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOutputServiceIsNull()
    {
        // Arrange
        var mockJokeService = new Mock<IJokeService>();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, mockJokeService.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenJokeServiceIsNull()
    {
        // Arrange
        var mockOutputService = new Mock<IOutput>();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(mockOutputService.Object, null!));
    }

    [Fact]
    public void TellJoke_SkipsChuckNorrisJoke_AndGetsNext()
    {
        // Arrange
        var mockJokeService = new Mock<IJokeService>();
        var mockOutputService = new Mock<IOutput>();

        var chuckJoke = "Chuck Norris always wins";
        var goodJoke = "A programmer walks into a bar";

        // Setup the mock to return Chuck Norris first, then a valid joke
        mockJokeService.SetupSequence(js => js.GetJoke())
            .Returns(chuckJoke) 
            .Returns(goodJoke);
            
        var jester = new Jester(mockOutputService.Object, mockJokeService.Object);

        // Act
        jester.TellJoke();

        // Assert
        mockJokeService.Verify(js => js.GetJoke(), Times.Exactly(2));
        mockOutputService.Verify(os => os.WriteLine(goodJoke), Times.Once);
        mockOutputService.Verify(os => os.WriteLine(chuckJoke), Times.Never);
    }
}

// ----------------------------------------------------------------------
// JOKE SERVICE TESTS (100% BRANCH COVERAGE)
// ----------------------------------------------------------------------

public class JokeServiceTests
{
    [Fact]
    public void GetJoke_ReturnsNonEmptyString()
    {
        // Arrange: Covers the successful try block
        var jokeService = new MockableJokeService(); 

        // Act
        var joke = jokeService.GetJoke();


        // Assert
        Assert.NotNull(joke);
        Assert.NotEmpty(joke);
    }

    [Fact]
    public void GetJoke_ChuckNorrisFilter()
    {
        // Test ensures JokeService returns the joke as-is (unfiltered)
        // Arrange
        var expectedJoke = "Chuck Norris joke";
        var jokeService = new MockableJokeService(nextJoke: expectedJoke);
        
        // Act
        var joke = jokeService.GetJoke();

        // Assert: JokeService must return the joke it received.
        Assert.Equal(expectedJoke, joke);
    }

    [Fact]
    public void GetJoke_ReturnsNoJokeFound_WhenApiJokeIsNull()
    {
        // NEW TEST: Covers the missing null coalescing branch (?? "No joke found")
        // Arrange: Pass null to MockableJokeService to simulate GetString() returning null
        var jokeService = new MockableJokeService(nextJoke: null);

        // Act
        var result = jokeService.GetJoke();

        // Assert
        Assert.Equal("No joke found", result);
    }

    [Fact]
    public void GetJoke_ReturnsErrorMessage_OnError()
    {
        // Arrange: Covers the catch block
        var jokeService = new MockableJokeService(shouldThrow: true);

        // Act
        var result = jokeService.GetJoke();

        // Assert
        Assert.Equal("Error retrieving joke. Please try again later", result);
    }
}

// ----------------------------------------------------------------------
// CONSOLE OUTPUT TESTS
// ----------------------------------------------------------------------

public class ConsoleOutputTests
{
    [Fact]
    public void WriteLine_FormatsMessageCorrectly()
    {
        // Arrange
        var outputService = new ConsoleOutput();
        var testMessage = "This is a test joke";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);


        // Act
        ((IOutput)outputService).WriteLine(testMessage);


        // Assert
        var output = stringWriter.ToString().Trim();
        Assert.Equal(testMessage, output);
    }

    [Fact]
    public void WriteLine_StoresOutputInPublicProperty()
    {
        // Arrange
        var outputService = new ConsoleOutput();
        var testMessage = "Another test";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);


        // Act
        ((IOutput)outputService).WriteLine(testMessage);


        // Assert
        Assert.Equal(testMessage, outputService.Output);
    }
}

// ----------------------------------------------------------------------
// INTEGRATION TESTS (Execute)
// ----------------------------------------------------------------------

public class IntegrationTests
{
    [Fact]
    public void JokeServiceImplementation_GetJoke_CallsUnderlyingJokeService()
    {
        // This test makes a real network call
        var jokeServiceImpl = new JokeService();


        // Act
        var joke = jokeServiceImpl.GetJoke();


        // Assert
        Assert.NotNull(joke);
        Assert.NotEmpty(joke);
    }

    [Fact]
    public void Jester_IntegrationTest_WithRealServices()
    {
        // This test makes a real network call
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        var outputService = new ConsoleOutput();
        var jokeService = new JokeService(); 
        var jester = new Jester(outputService, jokeService);


        // Act
        jester.TellJoke();


        // Assert
        var consoleOutput = stringWriter.ToString().Trim();
        Assert.NotEmpty(outputService.Output!);
    }
}