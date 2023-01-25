# Glob

[![Join the chat at https://gitter.im/kthompson/glob](https://badges.gitter.im/kthompson/glob.svg)](https://gitter.im/kthompson/glob?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build status](https://img.shields.io/azure-devops/build/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Coverage](https://img.shields.io/azure-devops/coverage/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Tests](https://img.shields.io/azure-devops/tests/automaters/Glob/7/develop)](https://automaters.visualstudio.com/Glob/_build?definitionId=7&branchFilter=51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51%2C51)
[![Nuget](https://img.shields.io/nuget/v/glob.svg)](https://www.nuget.org/packages/Glob/)


A C# Glob library for .NET.

## What is a glob?

A glob is a pattern-matching syntax that shells use.  Like when you do
`rm *.cs`, the `*.cs` is a glob. 

See: http://en.wikipedia.org/wiki/Glob_(programming) for more info.

## Try it out!

You can test out Glob expressions using this library in your browser by visiting:

https://kthompson.github.io/glob/


[![image](https://user-images.githubusercontent.com/15068/129584238-9afa3196-e7a5-4a76-9c5e-0db0377d8ad9.png)](https://kthompson.github.io/glob/)

## Supported Environments

* Windows
* Macintosh OS X (Darwin)
* Linux

## Features

### Common Expressions

| Pattern     | Description                                 |
|-------------|---------------------------------------------|
| taco*       | matches any string beginning with taco      |
| \*taco\*    | matches any string containing taco          |
| *taco       | matches any string ending in taco           |
| *.[ch]      | matches any string ending in `.c` or `.h`   |
| *.{gif,jpg} | match any string ending in `.gif` or `.jpg` |

### Expressions

| Pattern   | Description                                                                    |
|-----------|--------------------------------------------------------------------------------|
| *         | matches any number of characters including none, excluding directory separator |
| ?         | matches a single character                                                     |
| [abc]     | matches one character in the brackets                                          |
| [!abc]    | matches any character not in the brackets                                      |
| **        | match zero or more directories                                                 |
| {abc,123} | comma delimited set of literals, matched 'abc' or '123'                        |

### Other Features

* Escape patterns are supported using `\`
* Pure C# implementation
* No reliance on Regex
* Simple text string matching support
* File system matching APIs

## Getting Started

### Installing from NuGet

```bash
dotnet add package Glob
```

### Setup

To use Glob, you need to include the namespace:

```csharp
using GlobExpressions;
```

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

## Upgrading from 1.x

In 2.x all Glob expressions no longer support `\` as path separators. Instead `/` should be used to separate paths in expressions. 
The `/` path separator will still match on platform specific directory separators but `\` is reserved for escape sequences.


## Performance

[See benchmarks](test/Glob.Benchmarks/BenchmarkDotNet.Artifacts/results/GlobExpressions.Benchmarks.GlobBenchmarks-report-github.md)
