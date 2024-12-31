```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2605)
Unknown processor
.NET SDK 9.0.200-preview.0.24575.35
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2


```
| Method                                  | Mean        | Error       | StdDev      | Median      |
|---------------------------------------- |------------:|------------:|------------:|------------:|
| ParseGlob                               |  3,155.9 ns |    63.04 ns |    55.88 ns |  3,169.7 ns |
| ParseAndCompileGlob                     |  2,992.5 ns |    59.68 ns |   120.56 ns |  2,961.8 ns |
| MatchForUncompiledGlob                  |  3,496.5 ns |    66.21 ns |    55.29 ns |  3,518.8 ns |
| MatchForCompiledGlob                    |    352.2 ns |     6.87 ns |    10.90 ns |    350.0 ns |
| MatchForUncompiledGlobDirectoryWildcard |  3,277.9 ns |    64.09 ns |    68.58 ns |  3,269.9 ns |
| MatchForCompiledGlobDirectoryWildcard   |    395.4 ns |    24.64 ns |    70.69 ns |    368.3 ns |
| BenchmarkParseToTree                    |  3,925.5 ns |   138.34 ns |   407.91 ns |  3,847.5 ns |
| PathTraversal                           | 29,093.0 ns | 1,146.25 ns | 3,361.76 ns | 29,794.2 ns |
