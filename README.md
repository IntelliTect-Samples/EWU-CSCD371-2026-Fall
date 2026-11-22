# Assignment 9 - Resilient People Enricher

## Overview

Download a CSV of people from one endpoint, look up each person's **city** and **state** from a second endpoint, and write a final CSV that contains only the people you could fully and correctly enrich. The purpose of this assignment is to combine **exception handling**, **async/await**, and **LINQ** while talking to services that misbehave on purpose.

Both endpoints are intentionally unreliable. Any request may succeed or fail in unexpected ways. The failure mode is random, so **the same program can pass one run and fail the next**. Your job is to make it succeed every time.

**No starting code is provided.** Create your own `.slnx` solution, class library, and test project. You may reuse your Assignment 7 and 8 models, but this assignment must build and test independently.

## DATES

- Homework is Due on Monday, November 23 at 11:59PM.
- Peer Review is due by Wednesday, November 25 at 11:59PM.
- Final Corrections are due by Monday, November 30 at 11:59PM.

## Reading

- Essential C#: Exception Handling, including [Guidelines for Exception Handling](https://essentialcsharp.com/guidelines-for-exception-handling#guidelines-for-exception-handling).
- Essential C#: Task-Based Asynchronous Pattern (`async`/`await`, `CancellationToken`, `Task.WhenAll`).
- [`HttpClient.GetStringAsync`](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient.getstringasync) and its related methods.
- [HttpClient Guidelines](https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines)
- Essential C#: LINQ with Standard Query Operators.
- Essential C#: Resource cleanup (`IDisposable`, `IAsyncDisposable`, `using` statements/declarations, and `try`/`finally`).
- Review the exception handling guidelines covered in class.

## The Endpoints

Base URL: `https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io`

| Endpoint | Returns |
| --- | --- |
| `GET /people/{user}` | A CSV of 100 people for the given `{user}`. Use your own name or ID. |
| `GET /people/zip/{zip}` | One CSV line with the city and state for a ZIP code, such as `"Spokane","WA"`. |

The people CSV has this header:

```text
FirstName,LastName,EmailAddress,ZipCode
```

Your final CSV adds the two looked-up columns, in this order:

```text
FirstName,LastName,EmailAddress,ZipCode,City,State
```

### What can go wrong

By default each request picks a failure mode at random. Most requests succeed, but either endpoint may fail with:

- **Invalid data**
- **Timeout**
- **Exception**

It is up to you to discover what each of these looks like and how to handle it. Treat every one of them as something that **will** happen.

While developing or testing, you can force a mode with `?code=None`, `?code=InvalidData`, `?code=Timeout`, or `?code=Exception`. Your finished program must **not** use `code`. It has to cope with the random behavior, so run it **many times** and confirm it always produces a correct file, not just once. A request that fails on one run may succeed on the next attempt.

## Instructions

### Project setup

- Create a `.slnx` solution containing a class library and a unit test project that references it. ❌✔
- Add a small console (or other) entry point so the program can run against the live endpoints. The unit tests must **not** call the live endpoints. ❌✔
- Use a single, reused `HttpClient` (or `IHttpClientFactory`). Do not create a new `HttpClient` per request. ❌✔

### Download and parse

- Download the people CSV asynchronously and parse each row into a person model. ❌✔
- Validate the data you receive from **both** endpoints. Decide, and document, what makes a person's data invalid. ❌✔
    NOTE: *What makes a person invalid* 
        The row doesn't have exactly 4 columns (a quoted comma counts as an extra column; rows are removed, not repaired)
        First or last name is empty
        The email isn't a plain address (`a@b.com`; display names like `Bob <a@b.com>` are rejected)
        The ZIP isn't exactly 5 digits
        The ZIP lookup never returns `"City","ST"` with a non-empty city and 2 uppercase letters within 8 attempts

- A person with bad data is **removed**, wherever in the process the problem is discovered. Do not attempt to repair it, and do not stop the whole run because of one bad person. ❌✔
- Report what was removed and why (for example, a summary of rejected rows and their reasons). Do not silently discard people. ❌✔

### Enrich with city and state

- For each valid person, call the location endpoint and attach the `City` and `State`. ❌✔
- Look up people **concurrently** (not one after another), but limit how many requests are in flight at once. ❌✔
- If a person's lookup fails for good, or returns bad data, remove that person from the final CSV. ❌✔

### Exception handling and resilience

- Honor a caller-supplied `CancellationToken`. ❌✔
- Catch only failures you can recover from. Let unexpected exceptions propagate with their original stack traces. ❌✔
- If the **people CSV itself** cannot be downloaded, fail the run with an exception. Do not write a partial or empty file as though it succeeded. ❌✔

### Async/await

- Use `async`/`await` end to end. Do not block with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`. ❌✔
- Async methods accept a `CancellationToken` and pass it down to the calls they make. ❌✔

### LINQ

- Use LINQ for the data shaping: filtering out invalid rows, projecting each person into its enriched form, grouping or counting rejection reasons, and producing the final CSV lines. ❌✔
- Keep side effects (HTTP calls, file writes) out of your LINQ queries. ❌✔

### Resource management (`IDisposable`)

We expect every disposable resource you create or are handed to be cleaned up deterministically, on the success path **and** on every failure, retry, timeout, and cancellation path.

- Make sure to Dispose what you own. ❌✔
- A failed run must not leave partial or temporary files behind. ❌✔

### Output

- Write the final CSV with the header above, containing only fully enriched, valid people. ❌✔

### Tests

- Write tests that cover **all** of the required conditions above, including every failure mode on both endpoints, retries, cancellation, bad data, and resource cleanup. ❌✔

## Extra Credit

- Add a circuit-breaker or backoff with jitter, and test it. ❌✔
- Use a tool to generate an HttpClient wrapper from the OpenAPI specification (Kiota, NSwag, and Swagger are things you want to research). Use only the generated wrapper in your application. The OpenAPI spec can be found at `https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io/openapi/v1.json`. ❌✔

## Fundamentals

- Target all projects at .NET 10 (`net10.0`) or .NET 11 (`net11.0`; you will need to fix the `global.json` if you go this route) and use the latest `LangVersion`. ❌✔
- Enable nullable reference types, .NET analyzers, and code style enforcement for all projects. ❌✔
- Ensure there are no build errors or warnings, including analyzer warnings. ❌✔
- All of the above should be unit tested. ❌✔
- Choose simplicity over complexity.
