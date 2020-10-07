# Glob

[![Join the chat at https://gitter.im/kthompson/glob](https://badges.gitter.im/kthompson/glob.svg)](https://gitter.im/kthompson/glob?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://img.shields.io/azure-devops/build/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Coverage](https://img.shields.io/azure-devops/coverage/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Tests](https://img.shields.io/azure-devops/tests/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Nuget](https://img.shields.io/nuget/v/glob.svg)](https://www.nuget.org/packages/Glob/)


A C# Glob library for .NET and .NET Core.


## What is a glob?

A glob is a pattern-matching syntax that shells use.  Like when you do
`rm *.cs`, the `*.cs` is a glob. 

See: http://en.wikipedia.org/wiki/Glob_(programming) for more info.

## Supported Environments

* Windows
* Macintosh OS X (Darwin)
* Linux

## Supported Pattern expressions

| Pattern   | Description                                                                    |
|-----------|--------------------------------------------------------------------------------|
| *         | matches any number of characters including none, excluding directory seperator |
| ?         | matches a single character                                                     |
| [abc]     | matches one character in the brackets                                          |
| [!abc]    | matches any character not in the brackets                                      |
| **        | match zero or more directories                                                 |
| {abc,123} | comma delimited set of literals, matched 'abc' or '123'                        |


## Usage

### Example

```csharp
var glob = new Glob("**/bin");
var match = glob.IsMatch(@"C:\files\bin\");
```

### Static Usage

#### Single file

```csharp
var match = Glob.IsMatch(@"C:\files\bin\", "**/bin");	
```

#### Files in a directory

```csharp
string[] matchingFiles = Glob.Files(@"C:\files\bin\", "**/bin").ToArray();	
```

#### Directories in a directory

```csharp
string[] matchingDirectories = Glob.Directories(@"C:\files\bin\", "**/bin").ToArray();	
```

## Extension Methods

### DirectoryInfo.GlobDirectories

Enumerate through all matching directories recursively.

#### Params

* pattern: String

#### Example

```csharp
var root = new DirectoryInfo(@"C:\");
var allBinFolders = root.GlobDirectories("**/bin");
```

### DirectoryInfo.GlobFiles

Enumerate through all matching files recursively.

#### Params

* pattern: String

#### Example

```csharp
var root = new DirectoryInfo(@"C:\");
var allDllFiles = root.GlobFiles("**/*.dll");
```

### DirectoryInfo.GlobFileSystemInfos

Enumerate through all matching files and folders recursively.

#### Params

* pattern: String

#### Example

```csharp
var root = new DirectoryInfo(@"C:\");
var allInfoFilesAndFolders = root.GlobFileSystemInfos("**/*info");
```

## Performance

[See benchmarks](test/Glob.Benchmarks/BenchmarkDotNet.Artifacts/results/GlobExpressions.Benchmarks.GlobBenchmarks-report-github.md)
