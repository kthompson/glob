``` ini

BenchmarkDotNet=v0.10.4, OS=Windows 10.0.15063
Processor=Intel Core i7-4790K CPU 4.00GHz (Haswell), ProcessorCount=8
Frequency=3897223 Hz, Resolution=256.5930 ns, Timer=TSC
dotnet cli version=1.1.0
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                     Method |          Mean |      Error |     StdDev | Scaled | ScaledSD |
 |--------------------------- |--------------:|-----------:|-----------:|-------:|---------:|
 |                  ParseGlob | 2,408.0587 ns | 15.7785 ns | 13.1757 ns |   0.70 |     0.01 |
 |        ParseAndCompileGlob | 2,445.5885 ns | 16.6519 ns | 15.5762 ns |   0.71 |     0.01 |
 | TestMatchForUncompiledGlob | 3,460.6207 ns | 25.3431 ns | 22.4660 ns |   1.00 |     0.00 |
 |        BenchmarkParseToLst | 2,629.4912 ns | 22.0370 ns | 19.5353 ns |   0.76 |     0.01 |
 |       BenchmarkParseToTree | 2,436.2178 ns | 46.9586 ns | 50.2451 ns |   0.70 |     0.01 |
