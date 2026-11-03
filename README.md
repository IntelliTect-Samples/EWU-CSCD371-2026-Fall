# Exception Handling Guidelines

Live-coding examples for the *Guidelines for Exception Handling* section of
[Essential C#](https://essentialcsharp.com/general-catch-block#general-catch-block).

| Project | Purpose |
| --- | --- |
| `ExceptionHandling.Starter` | The "before" code with `// TODO`s. Type the fixes live in class. |
| `ExceptionHandling.Reference` | The finished "after" code, with before and after side by side. |
| `ExceptionHandling.Shared` | Demo infrastructure: the menu and top-level handler, `Log`, `Output`, and the simulated `WeatherApi` and `AnotherProgram`. |

## Running

```powershell
dotnet run --project ExceptionHandling.Starter            # interactive menu
dotnet run --project ExceptionHandling.Reference -- 5     # run demo 5 directly
```

| # | Guideline | File |
| --- | --- | --- |
| 1 | Catch only the exceptions you can handle | `CatchOnlyWhatYouCanHandle.cs` |
| 2 | Don't hide (bury) exceptions you don't fully handle | `DontBuryExceptions.cs` |
| 3 | Use `System.Exception` and general catch blocks rarely | `CatchSystemExceptionRarely.cs` |
| 4 | Avoid exception reporting or logging lower in the call stack | `LogHighInTheCallStack.cs` |
| 5 | Use `throw;` rather than `throw exception;` | `ThrowVersusThrowException.cs` |
| 6 | Favor exception filters over catch-and-rethrow | `FiltersInsteadOfRethrow.cs` |
| 7 | Avoid throwing exceptions from exception filters | `DontThrowFromFilters.cs` |
| 8 | Avoid exception filter conditions that might change over time | `StableFilterConditions.cs` |
| 9 | Use caution when rethrowing different exceptions | `WrapWithInnerException.cs` |

See [INSTRUCTOR-NOTES.md](INSTRUCTOR-NOTES.md) for the one-hour walkthrough.
