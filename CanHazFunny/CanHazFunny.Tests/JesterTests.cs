using System;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_WithNullJokeService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, new JokeWrite()));
    }

    [Fact]
    public void Constructor_WithNullJokeWrite_ThrowsArgumentNullException()
    {
        var service = new Mock<IJokeService>().Object;

        Assert.Throws<ArgumentNullException>(() => new Jester(service, null!));
    }

    [Fact]
    public void TellJoke_FirstJokeAcceptable_WritesOnlyAcceptedJoke()
    {
        var serviceMock = new Mock<IJokeService>();
        serviceMock.Setup(service => service.GetJoke())
            .Returns("Acceptable");
        var writeMock = new Mock<IJokeWrite>();
        var jester = new Jester(serviceMock.Object, writeMock.Object);

        jester.TellJoke();

        serviceMock.Verify(service => service.GetJoke(),
            Times.Exactly(1));
        writeMock.Verify(write => write.WriteJoke(It.IsAny<string>()),
            Times.Once());
        writeMock.Verify(write => write.WriteJoke("Acceptable"),
            Times.Once());
    }

    [Fact]
    public void TellJoke_SecondJokeAcceptable_WritesOnlyAcceptedJoke()
    {
        var serviceMock = new Mock<IJokeService>();
        serviceMock.SetupSequence(service => service.GetJoke())
            .Returns("Chuck Norris")
            .Returns("Acceptable");
        var writeMock = new Mock<IJokeWrite>();
        var jester = new Jester(serviceMock.Object, writeMock.Object);

        jester.TellJoke();

        serviceMock.Verify(service => service.GetJoke(),
            Times.Exactly(2));
        writeMock.Verify(write => write.WriteJoke(It.IsAny<string>()),
            Times.Once());
        writeMock.Verify(write => write.WriteJoke("Acceptable"),
            Times.Once());
    }

    [Fact]
    public void TellJoke_FourthJokeAcceptable_WritesOnlyAcceptedJoke()
    {
        var serviceMock = new Mock<IJokeService>();
        serviceMock.SetupSequence(service => service.GetJoke())
            .Returns("Chuck Norris")
            .Returns("Chuck Norris")
            .Returns("Chuck Norris")
            .Returns("Acceptable");
        var writeMock = new Mock<IJokeWrite>();
        var jester = new Jester(serviceMock.Object, writeMock.Object);

        jester.TellJoke();

        serviceMock.Verify(service => service.GetJoke(),
            Times.Exactly(4));
        writeMock.Verify(write => write.WriteJoke(It.IsAny<string>()),
            Times.Once());
        writeMock.Verify(write => write.WriteJoke("Acceptable"),
            Times.Once());
    }

    [Fact]
    public void TellJoke_JokeContainsChuckNorris_WritesOnlyAcceptedJoke()
    {
        var serviceMock = new Mock<IJokeService>();
        serviceMock.SetupSequence(service => service.GetJoke())
            .Returns("That Chuck Norris guy is acceptable")
            .Returns("Acceptable");
        var writeMock = new Mock<IJokeWrite>();
        var jester = new Jester(serviceMock.Object, writeMock.Object);

        jester.TellJoke();

        serviceMock.Verify(service => service.GetJoke(),
            Times.Exactly(2));
        writeMock.Verify(write => write.WriteJoke(It.IsAny<string>()),
            Times.Once());
        writeMock.Verify(write => write.WriteJoke("Acceptable"),
            Times.Once());
    }
}
