# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.5]
### Fixed
- Issue #55: Remove debug prints

## [1.1.4]
### Changed
- Removed reference to System.ValueTuple

### Fixed
- Issue #53: Fix issue where matches against the root directory would attempt to truncate a prefix longer than desired

## [1.1.3]
### Fixed
- Allow spaces as normal path characters

## [1.1.2]
### Fixed
- Issue #47: Add NuGet package description
- Issue #46: Add netstandard2.0 target
- Issue #34: Updated scanner to support `~` and `$` in Glob patterns
- Issue #44: Delete foreach Glob.Directories() fails

## [1.1.1]
### Fixed
- Issue with GlobOptions not having proper bitwise values

## [1.1.0]
### Added
- Added GlobOptions.MatchFullPath to require the full path to match for patterns like `*.txt`

### Changed
- Improved character range mechanics, to match ranges with `[`,`]`, `*`, and `?` in them

## [1.0.4]
### Changed
- Reduce number of allocations for matching Identifiers (small perf boost)

## [1.0.3]
### Fixed
- Fix Glob for `a/**` to match `a/` but not `a`
- Fix issue where roots (C:) would not obey the case sensitivity setting

## [1.0.2]
### Fixed
- Fix issue where matching `a/**` against `a` would cause a stackoverflow (#39)

## [1.0.1]
### Fixed
- Fix bug where `**` does not properly match `a` or `a/b` (#38)

## [1.0.0]
### Added
- Add Glob.Files and Glob.Directories APIs
- Support for Case-Insensitive globbing
- Reduced string allocations in Scanner

### Changed
- All types are now located in the GlobExpressions namespace instead of Glob
- Path traversal has been updated which may cause a different ordering of returned results

### Fixed
- Fix bug where not all files are returned when using GlobExtensions
- Fix NullReferenceException for empty patterns
- Fix issue where directory wildcard and identifier cannot be mixed in a single Segment
- Fix issue that caused unnecessary directory traversal (#20)
- Fix issue where Glob.Directories did not always match properly

[Unreleased]: https://github.com/kthompson/glob/compare/1.1.5...HEAD
[1.1.5]: https://github.com/kthompson/glob/compare/1.1.4...1.1.5
[1.1.4]: https://github.com/kthompson/glob/compare/1.1.3...1.1.4
[1.1.3]: https://github.com/kthompson/glob/compare/1.1.2...1.1.3
[1.1.2]: https://github.com/kthompson/glob/compare/1.1.1...1.1.2
[1.1.1]: https://github.com/kthompson/glob/compare/1.1.0...1.1.1
[1.1.0]: https://github.com/kthompson/glob/compare/1.0.4...1.1.0
[1.0.4]: https://github.com/kthompson/glob/compare/1.0.3...1.0.4
[1.0.3]: https://github.com/kthompson/glob/compare/1.0.2...1.0.3
[1.0.2]: https://github.com/kthompson/glob/compare/1.0.1...1.0.2
[1.0.1]: https://github.com/kthompson/glob/compare/1.0.0...1.0.1
[1.0.0]: https://github.com/kthompson/glob/compare/0.4.0...1.0.0
[0.4.0]: https://github.com/kthompson/glob/compare/0.3.3...0.4.0
[0.3.3]: https://github.com/kthompson/glob/compare/0.3.2...0.3.3
[0.3.2]: https://github.com/kthompson/glob/compare/0.3.1...0.3.2
[0.3.1]: https://github.com/kthompson/glob/compare/0.3.0...0.3.1
[0.3.0]: https://github.com/kthompson/glob/compare/0.2.1...0.3.0
[0.2.1]: https://github.com/kthompson/glob/compare/0.2.0...0.2.1
[0.2.0]: https://github.com/kthompson/glob/compare/0.1.0...0.2.0
