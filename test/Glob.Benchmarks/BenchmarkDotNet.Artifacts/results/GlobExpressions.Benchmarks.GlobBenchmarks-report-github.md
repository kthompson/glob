``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1049 (1909/November2018Update/19H2)
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|                                  Method |        Mean |     Error |    StdDev |
|---------------------------------------- |------------:|----------:|----------:|
|                               ParseGlob |  2,390.6 ns |  17.38 ns |  16.25 ns |
|                     ParseAndCompileGlob |  2,361.8 ns |  23.76 ns |  22.23 ns |
|                  MatchForUncompiledGlob |  2,870.3 ns |  30.13 ns |  28.18 ns |
|                    MatchForCompiledGlob |    434.6 ns |   2.23 ns |   1.86 ns |
| MatchForUncompiledGlobDirectoryWildcard |  2,805.2 ns |  28.22 ns |  26.39 ns |
|   MatchForCompiledGlobDirectoryWildcard |    438.0 ns |   5.45 ns |   5.10 ns |
|                    BenchmarkParseToTree |  2,349.2 ns |  27.50 ns |  24.37 ns |
|                           PathTraversal | 34,314.2 ns | 358.98 ns | 335.79 ns |
