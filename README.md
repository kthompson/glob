## What is a glob?

A glob is a pattern-matching syntax that shells use.  Like when you do
`rm *.cs`, the `*.cs` is a glob. 

See: http://en.wikipedia.org/wiki/Glob_(programming) for more info.

## Supported Environments

* Windows
* Macintosh OS X (Darwin)
* FreeBSD
* NetBSD
* Linux
* Solaris


## Why another glob library?

From all of my searching I have not been able to find a glob utility that works on Windows and *nix.
If you need something that works on all platforms... This is what you need.

This is also a pure C# implementation.


## Usage

    TBD

## Methods

### glob

Search through the filesystem asynchronously.

#### Params

* pattern: String
* flags: Optional - currently is not used but its here to work as a drop-in replacement
* cb: function

#### Example

    TBD

### fnmatch

Test if a string matches a pattern. (no i/o performed)

#### Params

* pattern: String
* str: String to test

#### Example

    TBD