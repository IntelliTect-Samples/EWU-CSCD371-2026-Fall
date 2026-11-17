# Synchronization samples

Requires the .NET 10 SDK or newer. No project files or additional packages.

```powershell
Set-Location .\Examples\Synchronization
dotnet run .\00-TaskConstructor.cs
```

Present the files in numeric order. Each file is independent; its teaching notes
are in the `Teaching notes` region at the bottom.

`11-Deadlock.cs` can hang depending on scheduling. Rerun if it completes;
stop it with **Ctrl+C** when it deadlocks.
