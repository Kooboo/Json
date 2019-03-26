``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.505
  [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  Job-XEQPPS : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT

IterationCount=100  LaunchCount=1  WarmupCount=1  

```
|           Method |       Mean |     Error |     StdDev |     Median |
|----------------- |-----------:|----------:|-----------:|-----------:|
|      ArrayToJson |  90.445 us | 4.9759 us | 14.6717 us |  83.600 us |
| DictionaryToJson |  33.951 us | 0.6963 us |  1.9977 us |  33.758 us |
|       ListToJson |  65.985 us | 2.3068 us |  6.6923 us |  66.302 us |
|     EntityToJson |   1.115 us | 0.0226 us |  0.0641 us |   1.098 us |
|      JsonToArray | 112.765 us | 5.2491 us | 15.4770 us | 116.357 us |
| JsonToDictionary |  77.239 us | 3.9610 us | 11.6791 us |  70.705 us |
|       JsonToList | 125.516 us | 7.9377 us | 23.2799 us | 113.611 us |
|     JsonToEntity |   2.178 us | 0.0325 us |  0.0896 us |   2.147 us |
