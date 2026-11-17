# Assignment 8 - Resilient People Importer

## Overview

Build a people CSV importer using the same format as Assignment 7. The purpose of this assignment is to learn **exception handling** and **resource management with `IDisposable`**. Your importer will either stop at the first invalid record or skip and report invalid records, and it must release its resources even when an exception occurs.

**No starting code is provided.** You may reuse your own Assignment 7 models (`Person`/`Address`), but this assignment must build and test independently.

## Reading

- Essential C#: Exception Handling, including [Guidelines for Exception Handling](https://essentialcsharp.com/guidelines-for-exception-handling#guidelines-for-exception-handling).
- Essential C#: Resource cleanup (`IDisposable`, `using` statements/declarations, and `try`/`finally`).
- Review the exception handling guidelines covered in class.

## Instructions

### Project setup

- Create a `.slnx` solution containing a class library and a unit test project that references it. ❌✔

### CSV data

Use the same columns as Assignment 7:

```text
Id,FirstName,LastName,Email,StreetAddress,City,State,Zip
```

- Parse each record into person/address models. ❌✔
- Validation requires exactly eight fields, a positive-integer `Id`, and nonblank person and address values.
- Create test data with a header and one record per line. You do not need to support quoted fields, embedded commas, or multiline records. ❌✔

### Exception handling

- Validate public method and constructor arguments. Throw `ArgumentNullException`, `ArgumentException`, or `ArgumentOutOfRangeException` as appropriate. Ensure the appropriate argument name is set on the thrown exception. Consider helpers such as `ArgumentNullException.ThrowIfNull` and `nameof`. ❌✔
- Define a custom exception, such as `InvalidPersonRecordException`, with a meaningful message, line number, and raw line text. Provide the standard constructors, including one that accepts an inner exception. ❌✔
- Wrap parsing failures in your custom exception, preserving the original exception as `InnerException`. ❌✔
- Let the caller choose between two import modes: ❌✔
  - **Fail-fast:** Throw your custom exception on the first invalid record. ❌✔
  - **Skip-invalid:** Report each invalid record and its exception through a caller-supplied callback, then continue. Do not silently discard records. ❌✔
- Catch only record errors you can recover from. Let `IOException`, callback exceptions, and other unexpected exceptions propagate with their original stack traces. Do not use `throw ex;` or hide failures in a general catch block. Favor exception filters over catch-and-rethrow. ❌✔
- Return the accepted people only after the import completes. A failed import must throw, not return a partial result as though it succeeded. ❌✔

### Resource management (`IDisposable`)

- Create a sealed component that implements `IDisposable` and owns a `TextReader`. Document ownership of any reader passed to it. A finalizer is not required. ❌✔
- Use `using` or `try`/`finally` to release owned resources after both successful and failed imports. ❌✔
- Make repeated `Dispose()` calls safe. Using the component after disposal must throw `ObjectDisposedException`. ❌✔

### Tests

- Test successful imports, including one from a temporary file. Delete the file when the test finishes. ❌✔
- Test both import modes with several kinds of invalid records. Verify fail-fast reports the first bad line number, and skip-invalid returns only valid people and reports every rejected record. Use a closure to collect or count callback reports. ❌✔
- Verify wrapped exceptions preserve the expected `InnerException` type, and argument exceptions identify the correct parameter. ❌✔
- Use a fake `TextReader` or a mocking library to simulate an I/O failure. Verify reader and callback exceptions propagate as the same exception instance, without wrapping or swallowing them. ❌✔
- Test disposal after successful and failed imports, repeated disposal, and use after disposal. ❌✔

## Extra Credit

- Support a caller-supplied validation delegate. Handle rule violations through the selected import mode, but let unexpected exceptions from the delegate propagate. ❌✔
- Use a `when` filter to handle specific recoverable conditions. Include a test showing that non-matching exceptions are not caught. ❌✔

## Fundamentals

- Target all projects at .NET 10 (`net10.0`) and use the latest `LangVersion`. ❌✔
- Enable nullable reference types, .NET analyzers, and code style enforcement for all projects. ❌✔
- Ensure there are no build errors or warnings, including analyzer warnings. ❌✔
- All of the above should be unit tested. ❌✔
- Choose simplicity over complexity. No console UI, async, or multithreading is required. ❌✔

