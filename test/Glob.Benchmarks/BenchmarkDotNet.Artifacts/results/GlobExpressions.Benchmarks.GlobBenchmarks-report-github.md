``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                                  Method |        Mean |     Error |    StdDev |      Median |
|---------------------------------------- |------------:|----------:|----------:|------------:|
|                               ParseGlob |  1,962.7 ns |  28.34 ns |  26.50 ns |  1,957.7 ns |
|                     ParseAndCompileGlob |  2,012.8 ns |  39.59 ns |  37.04 ns |  2,015.1 ns |
|                  MatchForUncompiledGlob |  2,681.5 ns |  20.05 ns |  18.76 ns |  2,676.7 ns |
|                    MatchForCompiledGlob |    661.3 ns |  13.11 ns |  27.94 ns |    649.5 ns |
| MatchForUncompiledGlobDirectoryWildcard |  2,614.3 ns |  51.72 ns |  43.19 ns |  2,610.4 ns |
|   MatchForCompiledGlobDirectoryWildcard |    663.8 ns |  16.19 ns |  21.05 ns |    658.0 ns |
|                    BenchmarkParseToTree |  2,032.0 ns |  40.70 ns |  64.56 ns |  2,000.9 ns |
|                           PathTraversal | 48,384.5 ns | 209.39 ns | 195.86 ns | 48,345.2 ns |
