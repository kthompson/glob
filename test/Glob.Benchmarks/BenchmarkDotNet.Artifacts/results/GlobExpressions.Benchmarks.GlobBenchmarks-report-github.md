``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                        Method |           Mean |       Error |      StdDev |
|------------------------------ |---------------:|------------:|------------:|
| PathTraversalOldDirectoryInfo | 1,305,079.3 ns | 11,118.3 ns | 10,400.1 ns |
|                 PathTraversal |    45,903.0 ns |    258.9 ns |    229.5 ns |
