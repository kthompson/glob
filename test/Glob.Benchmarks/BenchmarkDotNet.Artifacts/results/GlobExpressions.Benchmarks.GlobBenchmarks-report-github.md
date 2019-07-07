``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17763
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.203
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|                                  Method |        Mean |      Error |     StdDev |
|---------------------------------------- |------------:|-----------:|-----------:|
|                               ParseGlob |  4,780.5 ns |  29.823 ns |  27.897 ns |
|                     ParseAndCompileGlob |  4,712.5 ns |  20.496 ns |  17.115 ns |
|                  MatchForUncompiledGlob |  5,386.6 ns |  42.365 ns |  39.628 ns |
|                    MatchForCompiledGlob |    599.5 ns |   5.919 ns |   5.537 ns |
| MatchForUncompiledGlobDirectoryWildcard |  5,245.5 ns |  42.856 ns |  40.088 ns |
|   MatchForCompiledGlobDirectoryWildcard |    595.4 ns |   4.899 ns |   4.582 ns |
|                    BenchmarkParseToTree |  4,707.7 ns |  50.401 ns |  47.145 ns |
|                           PathTraversal | 38,026.5 ns | 361.679 ns | 320.619 ns |
