``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.505
  [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  Job-XEQPPS : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT

IterationCount=100  LaunchCount=1  WarmupCount=1  

```
|           Method |      Mean |     Error |    StdDev |    Median |
|----------------- |----------:|----------:|----------:|----------:|
|      ArrayToJson | 39.312 us | 2.2085 us | 6.5119 us | 35.572 us |
| DictionaryToJson | 25.919 us | 0.1982 us | 0.5524 us | 25.827 us |
|       ListToJson | 41.170 us | 0.8671 us | 2.3294 us | 40.793 us |
|     EntityToJson |  1.145 us | 0.0147 us | 0.0415 us |  1.135 us |
|      JsonToArray | 67.553 us | 0.4472 us | 1.2830 us | 67.463 us |
| JsonToDictionary | 37.396 us | 0.2935 us | 0.8422 us | 37.391 us |
|       JsonToList | 59.472 us | 0.4560 us | 1.3375 us | 59.422 us |
|     JsonToEntity |  1.401 us | 0.0105 us | 0.0305 us |  1.398 us |
