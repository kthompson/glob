``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1049 (1909/November2018Update/19H2)
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|                                  Method |        Mean |     Error |    StdDev |
|---------------------------------------- |------------:|----------:|----------:|
|                               ParseGlob |  3,454.9 ns |  57.56 ns |  53.84 ns |
|                     ParseAndCompileGlob |  3,380.3 ns |  21.51 ns |  20.12 ns |
|                  MatchForUncompiledGlob |  3,829.5 ns |  34.29 ns |  30.40 ns |
|                    MatchForCompiledGlob |    450.7 ns |   1.79 ns |   1.67 ns |
| MatchForUncompiledGlobDirectoryWildcard |  3,777.2 ns |  52.82 ns |  46.82 ns |
|   MatchForCompiledGlobDirectoryWildcard |    452.0 ns |   2.17 ns |   2.03 ns |
|                    BenchmarkParseToTree |  3,304.1 ns |  21.61 ns |  19.16 ns |
|                           PathTraversal | 34,536.0 ns | 330.56 ns | 309.20 ns |
