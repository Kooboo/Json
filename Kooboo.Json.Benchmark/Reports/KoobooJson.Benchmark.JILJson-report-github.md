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
|      ArrayToJson | 47.057 us | 3.3739 us | 9.8419 us | 43.050 us |
| DictionaryToJson | 30.439 us | 0.8282 us | 2.3358 us | 30.150 us |
|       ListToJson | 39.714 us | 0.7294 us | 2.0572 us | 39.800 us |
|     EntityToJson |  5.735 us | 0.5437 us | 1.5775 us |  5.200 us |
|      JsonToArray | 79.824 us | 1.0492 us | 2.9421 us | 79.200 us |
| JsonToDictionary | 62.258 us | 1.4621 us | 4.1949 us | 60.800 us |
|       JsonToList | 90.837 us | 0.8821 us | 2.4879 us | 90.150 us |
|     JsonToEntity |  1.650 us | 0.0159 us | 0.0445 us |  1.631 us |
