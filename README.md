## What is a glob?

A glob is a pattern-matching syntax that shells use.  Like when you do
`rm *.cs`, the `*.cs` is a glob. 

See: http://en.wikipedia.org/wiki/Glob_(programming) for more info.

## Supported Environments

* Windows
* Macintosh OS X (Darwin)
* Linux


## Why another glob library?

From all of my searching I have not been able to find a glob utility that works on Windows and *nix.
If you need something that works on all platforms... This is what you need.

This is also a pure C# implementation.


## Usage

### Example

	var glob = new Glob("**/bin");
	var match = glob.IsMatch(@"C:\files\bin\");

### Static Usage

    var match = Glob.IsMatch(@"C:\files\bin\", "**/bin");	

## Build Status

[![.NET Build Status](https://img.shields.io/appveyor/ci/kthompson/csharp-glob/master.svg)](https://ci.appveyor.com/project/kthompson/csharp-glob)

[![Nuget](https://img.shields.io/nuget/v/csharp-glob.svg)](http://targetaddress.com)


## Extension Methods

### DirectoryInfo.GlobDirectories

Enumerate through all matching directories recursively.

#### Params

* pattern: String

#### Example

    var root = new DirectoryInfo("C:\");
	var allBinFolders = root.GlobDirectories("**/bin");

### DirectoryInfo.GlobFiles

Enumerate through all matching files recursively.

#### Params

* pattern: String

#### Example

    var root = new DirectoryInfo("C:\");
	var allDllFiles = root.GlobFiles("**/*.dll");

### DirectoryInfo.GlobFileSystemInfos

Enumerate through all matching files and folders recursively.

#### Params

* pattern: String

#### Example

    var root = new DirectoryInfo("C:\");
	var allInfoFilesAndFolders = root.GlobFileSystemInfos("**/*info");
