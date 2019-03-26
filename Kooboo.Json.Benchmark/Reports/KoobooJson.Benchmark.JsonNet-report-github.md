``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.505
  [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  Job-XEQPPS : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT

IterationCount=100  LaunchCount=1  WarmupCount=1  

```
|           Method |       Mean |     Error |    StdDev |     Median |
|----------------- |-----------:|----------:|----------:|-----------:|
|      ArrayToJson |  85.464 us | 0.5775 us | 1.6002 us |  85.269 us |
| DictionaryToJson |  56.422 us | 0.4093 us | 1.1610 us |  56.371 us |
|       ListToJson |  99.092 us | 2.3965 us | 6.7200 us |  97.915 us |
|     EntityToJson |   2.529 us | 0.0109 us | 0.0302 us |   2.521 us |
|      JsonToArray | 142.847 us | 1.6871 us | 4.9213 us | 142.484 us |
| JsonToDictionary |  99.744 us | 3.3349 us | 9.4061 us |  95.983 us |
|       JsonToList | 163.788 us | 1.8132 us | 5.2604 us | 162.800 us |
|     JsonToEntity |   5.333 us | 0.1347 us | 0.3731 us |   5.230 us |
