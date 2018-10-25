``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                     Method |       Mean |    Error |   StdDev |     Median | Scaled | ScaledSD |
|--------------------------- |-----------:|---------:|---------:|-----------:|-------:|---------:|
|                  ParseGlob | 2,437.0 ns | 48.34 ns | 108.1 ns | 2,393.1 ns |   0.74 |     0.04 |
|        ParseAndCompileGlob | 2,481.9 ns | 49.59 ns | 122.6 ns | 2,444.3 ns |   0.75 |     0.04 |
| TestMatchForUncompiledGlob | 3,313.3 ns | 65.77 ns | 115.2 ns | 3,338.7 ns |   1.00 |     0.00 |
|       BenchmarkParseToTree | 2,518.1 ns | 72.36 ns | 205.3 ns | 2,453.3 ns |   0.76 |     0.07 |
