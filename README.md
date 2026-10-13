# JASS Merge Studio

A Windows desktop text comparison and manual merge application built with C#,
.NET 10 and Avalonia UI.

## Current features
- Open two text or source files.
- Compare lines by position and review differences.
- Initialize a merge from either input.
- Edit and save merged output.

## Sample files
See `test-samples/` for pairs of TXT, JSON, CSV, Markdown and C# files.

## Build and run
Requires the .NET 10 SDK.

```powershell
dotnet restore
dotnet build
dotnet run
```

## Current limitations
The initial comparison engine compares lines by position. It does not yet
implement a full diff algorithm, automatic three-way merging, or
syntax-aware JSON/CSV merging.
