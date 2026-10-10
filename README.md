# Assignment 3 (EWU-CSCD371-2026-Fall)

## Overview

For this assignment we are going to create a joke generator. For fun we will back it with a real web service that provides jokes. However, Chuck Norris jokes are over-used so we are going to filter those out. The code to retrieve the jokes from the web service is provided for you.
**Please note** This is someone else's web service, please be respectful and avoid spamming it.

## Assignment

\- Create two interfaces. One to represent the `JokeService` and one interface to represent outputting the joke to the screen (`Console.WriteLine`). ✔

- `CanHazFunny/CanHazFunny/IJokeService.cs` defines the joke-service interface.
- `CanHazFunny/CanHazFunny/IJokeConsoleOutput.cs` defines the console-output interface.

\- The `JokeService` will need to have the interface applied to it. ✔

- `CanHazFunny/CanHazFunny/JokeService.cs` implements `IJokeService`.
- `CanHazFunny.Tests/JokeServiceTests.cs` tests JSON retrieval and response handling.

\- Create an implementation of the output interface that writes out the joke to the console. ✔

- `CanHazFunny/CanHazFunny/JokeConsoleOutput.cs` contains the `JokeOutput` class, which implements `IJokeConsoleOutput` and calls `Console.WriteLine`.
- `CanHazFunny.Tests/JokeConsoleOutputTests.cs` tests console output and null handling.

\- Implement the `Jester` class. It should take in both interfaces as dependencies. These dependencies should be null checked. ✔

- `CanHazFunny/CanHazFunny/Jester.cs` implements `Jester` and validates both constructor dependencies.
- `CanHazFunny.Tests/JesterTests.cs` tests null dependencies and verifies the expected exception parameter names.

\- The `Jester` class `TellJoke()` method should retrieve a joke from the `JokeService`. If the joke contains "Chuck Norris", skip it and get another. The joke should be written to the output dependency. ✔

- `CanHazFunny/CanHazFunny/Jester.cs` contains the retrieval, filtering, retry, and output logic.
- `CanHazFunny.Tests/JesterTests.cs` tests forbidden terms, capitalization variations, retry behavior, and output of the accepted joke. The filter also rejects jokes mentioning `Chuck`, `Norris`, or `Texas Ranger` independently.

\- Unit test the Jester class. Code coverage should be above 90% for this class. Moq will make this significantly easier. ✔

- `CanHazFunny/CanHazFunny.Tests/JesterTests.cs` uses Moq for the dependencies.
- The latest verified coverage report for `CanHazFunny.Jester` showed 100% line coverage and 100% branch coverage.
- All 13 test cases passed in the latest test run.

## Fundamentals

\- Target all projects at .NET 10 (`net10.0`). ✔

- The root `Directory.Build.props` sets the target framework for both projects.
- The clean build confirmed that both projects target `net10.0`.

\- Use xUnit v3 with Microsoft Testing Platform (MTP) for unit tests. ✔

- `CanHazFunny/CanHazFunny.Tests/CanHazFunny.Tests.csproj` contains the test-project configuration.
- The test runner successfully discovered and executed all 13 test cases.

\- Be sure you enable:

- Nullability for all projects ✔
  - `Directory.Build.props` sets `<Nullable>enable</Nullable>`.
- Set `LangVersion` and the `TargetFramework` to the latest released versions available (preview versions optional) ✔
  - `Directory.Build.props` sets `LangVersion` to `14.0` and `TargetFramework` to `net10.0`. C# 14 is the latest released C# version, and it is supported by .NET 10.&#x20;
- Ensure that you turn on code analysis for all projects (`EnableNETAnalyzers`) ✔
  - `Directory.Build.props` sets `<EnableNETAnalyzers>true</EnableNETAnalyzers>`.
- Ensure that you turn on treat warnings as errors for all projects ✔
  - `Directory.Build.props` sets `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.
- Ensure that you turn on `CodeAnalysisTreatWarningsAsErrors` ✔
  - `Directory.Build.props` sets `<CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>`.
- Ensure that you turn on `EnforceCodeStyleInBuild` ✔
  - `Directory.Build.props` sets `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>`.

\- Ensure there are no errors or warnings (including code analysis warnings) ✔

- `dotnet clean` and `dotnet build` both succeeded. The build output showed no warnings or errors.
- `dotnet test` passed all 13 test cases, with zero failures.

\- All of the above should be unit tested ✔

- `CanHazFunny/CanHazFunny.Tests/JesterTests.cs` tests `Jester`.
- `CanHazFunny/CanHazFunny.Tests/JokeConsoleOutputTests.cs` tests console output.
- `CanHazFunny/CanHazFunny.Tests/JokeServiceTests.cs` tests JSON parsing and invalid responses.
- The project configuration is verified by the successful clean build rather than behavioral unit tests.

\- Choose simplicity over complexity ✔

- The design uses two interfaces, their concrete implementations, and `Jester` to coordinate the behavior.
- The `JokeService` constructor accepting `HttpClient` enables isolated tests without changing the normal parameterless application setup.

## Extra Credit

\- Unit test your implementation that writes the joke out to the screen. How hard could it be to unit test a single line method ;) ? ✔

- `CanHazFunny/CanHazFunny.Tests/JokeConsoleOutputTests.cs` captures `Console.Out` and verifies the output.
- Both console-output tests passed.

\- The Geek jokes API that is being used can also return jokes in a JSON format. Update the `JokeService` to retrieve jokes using JSON. `GetJoke` should still return a string. ✔

- `CanHazFunny/CanHazFunny/JokeService.cs` requests the JSON endpoint, extracts the `joke` property, and returns a `string`.
- `CanHazFunny/CanHazFunny.Tests/JokeServiceTests.cs` tests the endpoint, JSON extraction, Unicode escape decoding, invalid JSON, missing joke values, and a null HTTP client.
- The application was run successfully after the JSON change and displayed a joke.

## Additional links

- [coverlet](https://github.com/coverlet-coverage/coverlet#Quick-Start) code coverage tool
- [Geek jokes API](https://github.com/sameerkumar18/geek-joke-api)

## See [Docs](https://github.com/IntelliTect-Samples/EWU-CSCD371-2026-Fall/blob/main/Docs/README.md)
