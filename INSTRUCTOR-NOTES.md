# Instructor Notes: Exception Handling Guidelines (≈ 60 minutes)

**Setup:** Open `ExceptionHandling.slnx`. Type live in **ExceptionHandling.Starter**. Each starter
file contains the "before" code plus `// TODO`s. Keep **ExceptionHandling.Reference** open on a
second screen as your answer key.

Run a demo with `dotnet run --project ExceptionHandling.Starter -- <n>`, or use the menu.

Rhythm for each demo: **Run → ask the class to predict → type the fix → run again → state the guideline.**

| Time | Demo |
| --- | --- |
| 0:00 | Opener: general catch block (slides or whiteboard) |
| 0:03 | 1. Catch only what you can handle |
| 0:10 | 2. Don't bury exceptions |
| 0:15 | 3. `System.Exception` and `catch { }` rarely |
| 0:20 | 4. Log high in the call stack |
| 0:26 | 5. `throw;` vs `throw exception;` |
| 0:32 | 6. Filters instead of rethrow |
| 0:40 | 7. Don't throw from filters |
| 0:46 | 8. Filter conditions that change over time |
| 0:50 | 9. Rethrowing different exceptions |
| 0:57 | Wrap-up |

---

## 0:00 Opener: the general catch block (3 min)

- `catch { }` behaves like `catch (Exception)`, except it has no exception variable, and it must come last.
- Since C# 2.0, anything thrown by another language that doesn't derive from `Exception`
  (C++ can `throw "oops";`) arrives wrapped in `RuntimeWrappedException`. So `catch (Exception)`
  already catches **everything**.
- In IL, `catch { }` is actually `catch (object)`, which you can't write in C#.
- **Optional live type:** put `catch (Exception) { }` followed by `catch { }` in one try statement and show
  warning **CS1058**: "A previous catch clause already catches all exceptions."
- Segue: "So we *can* catch everything. Today is about when we *should*."

## 1. Catch only the exceptions you can handle (7 min)

> CONSIDER catching a specific exception when you understand why it was thrown and can respond to it programmatically.

1. **Run 1.** It crashes with `IOException: ... being used by another process`.
2. **Ask:** "Can *this* method do anything useful about a locked file?" (Yes: wait and retry.)
3. **Type** the retry loop:

   ```csharp
   for (int attempt = 1; attempt <= maxAttempts; attempt++)
   {
       try
       {
           using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
           using StreamReader reader = new(stream);
           return reader.ReadToEnd();
       }
       catch (IOException)
       {
           Output.Note($"  Attempt {attempt} of {maxAttempts}: file is in use, waiting...");
           Thread.Sleep(250);
       }
   }
   return null;
   ```

4. **Run 1. Predict:** what happens with `no-such-report.txt`?
   It retries three times and then tells the user to *close it in another program*. That's wrong.
   `FileNotFoundException` **derives from** `IOException`. Our catch was broader than our knowledge.
5. **Fix:** `catch (IOException exception) when (exception is not FileNotFoundException and not DirectoryNotFoundException)`
   (filters are covered fully in demo 6). Rerun: the missing file now reaches the top-level handler. ✔
- **Point:** Catching is a promise that you know the *recovery action*. "Show a message" isn't recovery.
  Other exceptions (`UnauthorizedAccessException`, `ArgumentException`) keep flowing to callers who might know what to do.

## 2. Don't hide (bury) exceptions you don't fully handle (5 min)

> DO NOT over-catch. Exceptions should be allowed to propagate up the call stack unless it is clearly understood how to programmatically address those errors lower in the stack.

1. **Run 2.** It says "Settings saved!" and then `File exists? False`. **Ask:** "When does the user find out?" (When their settings are gone, with no log and no clue why.)
2. **Type** `DeleteFolder` with `catch (DirectoryNotFoundException) { }`.
   This is an **explicit, documented** decision that this one exception is innocuous: the folder being gone is the goal.
3. **Type** `SaveSettings` with **no** try/catch, call it, and rerun. Now the top-level handler reports `DirectoryNotFoundException`. ✔
- **Point:** An empty catch should be specific *and* commented, explaining why it's safe. `catch { }` around "everything" never is.

## 3. Use System.Exception and general catch blocks rarely (5 min)

> AVOID catching `System.Exception` or `System.SystemException` except in top-level exception handlers that perform final cleanup operations before rethrowing the exception.

1. **Run 3.** It crashes on order 1003. Show `%TEMP%\orders-export.csv`: a **half-written** export is left behind.
2. **Type:**

   ```csharp
   try
   {
       ExportOrders(exportPath, orders);
   }
   catch
   {
       File.Delete(exportPath);
       throw;
   }
   ```

3. **Ask:** "Why is a general catch OK here?" Because it doesn't *handle* anything. It cleans up and **rethrows**.
   (Why not `finally`? Because on success we want to keep the file.)
4. Show the other legitimate place: `DemoRunner.RunDemo` in `ExceptionHandling.Shared`, the **top-level handler**.
- **Accuracy note (the book is a bit dated here):** On modern .NET, `StackOverflowException` can't be
  caught at all; the process terminates. Corrupted-state exceptions such as `AccessViolationException`
  aren't delivered to catch blocks either. `OutOfMemoryException` *can* be caught, but there is rarely a
  safe recovery. The conclusion still holds: these failures shouldn't be "handled," so don't write catch-alls that pretend to.

## 4. Avoid exception reporting or logging lower in the call stack (6 min)

> AVOID exception reporting or logging lower in the call stack.

1. **Run 4.** **Ask first:** "How many log entries for ONE failure?" The answer is **3**, and one `Console.WriteLine` comes from the business layer.
2. **Ask:** "Where does `Console.WriteLine` go in a web API? A Windows service?" (Nowhere anyone sees.)
3. **Type** `GreetCustomer(int id) => $"Hello, {FindCustomerName(id)}!";` and duplicate the `Run` block. Now there is **1** entry. ✔
- **Point:** Lower layers don't know the app type (console, web, service) or the audience.
  The top of the stack does. Log once, with full context, at the boundary.
- **Note:** In ASP.NET Core this boundary is middleware or `IExceptionHandler`. In a console app, it's `Main` (our `DemoRunner`).

## 5. Use `throw;` rather than `throw exception;` (6 min)

> DO use `throw;` rather than `throw <exception object>` inside a catch block.

1. **Run 5.** Both stack traces start at the *catch* method. **Ask:** "Where did it really fail?" (`ParseAge`, which isn't in either trace.)
2. Point out the `#pragma warning disable CA2200`: **the analyzer already warns** about this mistake.
3. **Fix** `ParseAgeWithThrow`: change `catch (FormatException exception)` to `catch (FormatException)`, change the throw to `throw;`, and delete the pragmas.
4. **Rerun.** The trace now begins at `ParseAge` where the exception was thrown, followed by the rethrowing method and `Run`. ✔
- **Point:** `throw exception;` resets the stack trace to the line where you rethrew.
  `throw;` preserves the original throw point, which is the thing you need at 2 a.m.
- **Aside:** To rethrow *outside* the catch (another thread, or later), use
  `ExceptionDispatchInfo.Capture(exception).Throw();`.

## 6. Favor exception filters to avoid rethrowing inside a catch (8 min)

> DO use exception filters to avoid rethrowing an exception from within a catch block.

1. **Run 6.** Read `GetForecastWithRethrow` together: catch, `if (not 404) throw;`. **Ask:** "Did we actually handle the 503?" (No. We caught it only to throw it back.)
2. **Type:**

   ```csharp
   private static string GetForecastOrDefault(string city)
   {
       try
       {
           return CallWeatherApi(city);
       }
       catch (HttpRequestException exception) when (IsNotFound(exception))
       {
           return "No forecast available.";
       }
   }
   ```

   Then add `Report(() => GetForecastOrDefault("Overloaded"));`.
3. **Run and compare the order of the gray lines:**
   - Rethrow: `finally: connection closed` → `catch block entered`. The stack has **already unwound**.
   - Filter: `filter evaluated` → `finally: connection closed`. The filter runs **before** unwinding.
- **Why it matters:** .NET handles exceptions in two passes. First it *searches* for a handler by evaluating filters,
  then it *unwinds* (running `finally` blocks). With a filter, an exception you don't handle is never caught,
  so the debugger and crash dumps stop at the **original throw site** with all locals intact.
- Filters can use any boolean expression: `when (exception.StatusCode == HttpStatusCode.NotFound)`, `when (retries < 3)`, and so on.

## 7. Avoid throwing exceptions from exception filters (6 min)

> AVOID throwing exceptions from exception filters.

1. **Run 7.** `Overloaded` is handled, but `Offline` is **not**. **Ask:** "Why? Our filter handles 'never reached the server'... doesn't it?"
2. Show `IsTransient`: `StatusCode` is **null** for `Offline`, so `.Value` throws `InvalidOperationException`.
3. **Ask:** "So where's the `InvalidOperationException`?" **Gone.** An exception thrown inside a filter is
   swallowed and the filter counts as `false`. Nothing gets logged and nothing crashes; the logic is just silently wrong.
4. Point at the `!`: the compiler warned us (CS8629), and we silenced it.
5. **Fix:** `exception.StatusCode is null or >= HttpStatusCode.InternalServerError`. Rerun. ✔
- **Point:** Keep filters simple and non-throwing. If a check is complicated, move it to a method
  with its own explicit try/catch that decides what to return.

## 8. Avoid exception filter conditions that might change over time (4 min)

> AVOID exception filters with logic that might implicitly change over time.

1. **Run 8.** v1 works and v2 is **NOT handled**. The vendor reworded "not found" to "No forecast is available." No compile error and no failing test, but the behavior changed.
2. **Type** `GetForecastByStatusCode` with `when (exception.StatusCode == HttpStatusCode.NotFound)` and add its line. Both versions now work. ✔
- **Point:** Messages are for humans. They get reworded, **localized** (by `CurrentUICulture`), and recapitalized.
  Filter on the *contract*: exception type, `StatusCode`, `ParamName`, `SocketErrorCode`, documented `HResult`s, and so on.

## 9. Use caution when rethrowing different exceptions (7 min)

> DO use caution when rethrowing different exceptions.

1. **Run 9.** The output says `Setting 'timeout' is invalid.` and `(no InnerException - the original cause is gone)`.
   **Ask:** "Invalid how? Should I look at the value? The file? Permissions?"
2. **Type:** `catch (FormatException exception)`, then
   `throw new InvalidSettingsException($"Setting '{parts[0]}' must be a whole number but was '{parts[1]}'.", exception);`
3. **Rerun.** Now the `---> FormatException` chain is visible. ✔ `exception.ToString()` prints the full chain, including inner stack traces.
- **Point:** Throwing a new exception resets the stack **and** hides the original unless you pass it as `innerException`.
- **When is rethrowing a *different* exception appropriate?** (The website's list is cut off; these are the book's three situations.)
  1. **Changing the exception type clarifies the problem.** `FormatException` means little to the caller;
     `InvalidSettingsException("timeout must be a whole number")` means a lot.
  2. **Private data is part of the original exception.** Example: a database exception whose message includes a
     connection string or password. Wrap it in a sanitized exception, and *consider deliberately omitting* the inner
     exception (log the original securely at the boundary instead).
  3. **The exception type is too specific for the caller to handle appropriately.** A caller of `ISettingsStore`
     shouldn't need to catch `FileNotFoundException` *and* `SqlException` *and* `HttpRequestException` depending on the
     backing store. Translate them to one abstraction-level exception and keep the original as the inner exception.
- Also notice the custom exception: its name ends in `Exception`, it derives from `Exception`, and it provides the three standard constructors.

## 0:57 Wrap-up (3 min)

Ask the class to sort these into **catch here** vs **let it propagate**:

- `IOException` from a locked file, in a method that can retry → **catch** (demo 1)
- `FileNotFoundException`, in a method that can't conjure files → **propagate** (demo 1)
- "Any exception" in a data-access method that just wants to log → **propagate; log at the top** (demo 4)
- "Any exception," when a half-written file must be deleted → **catch, clean up, `throw;`** (demo 3)
- A 404 from a web service when "no data" is a valid answer → **catch with a filter** (demo 6)

**One-line summary:** *Catch narrowly, filter on stable data, clean up and `throw;`, and log once at the top.*
