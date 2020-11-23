# ChannelPlayground


``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-6700HQ CPU 2.60GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  Job-GLCAZR : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|             Method | PublisherCardinality | SubscriberCardinality |        Type | AllowSyncContinuations |      Mean |     Error |    StdDev |    Median |   Op/s | Ratio |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|------------------- |--------------------- |---------------------- |------------ |----------------------- |----------:|----------:|----------:|----------:|-------:|------:|----------:|----------:|----------:|----------:|
|        **ChannelPerf** |               **Single** |                **Single** | **BoundedWait** |                   **True** | **69.520 ms** | **1.3340 ms** | **1.2478 ms** | **69.594 ms** |  **14.38** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** | **8392272 B** |
|      DisruptorPerf |               Single |                Single | BoundedWait |                   True |  2.886 ms | 0.0634 ms | 0.1810 ms |  2.885 ms | 346.53 |  0.04 |         - |         - |         - |     504 B |
| ValueDisruptorPerf |               Single |                Single | BoundedWait |                   True |  2.194 ms | 0.0863 ms | 0.2503 ms |  2.125 ms | 455.75 |  0.03 |         - |         - |         - |     504 B |
|                    |                      |                       |             |                        |           |           |           |           |        |       |           |           |           |           |
|        **ChannelPerf** |                **Multi** |                **Single** | **BoundedWait** |                   **True** | **69.180 ms** | **1.3653 ms** | **1.6253 ms** | **68.438 ms** |  **14.46** |  **1.00** | **1000.0000** | **1000.0000** | **1000.0000** | **8394664 B** |
|      DisruptorPerf |                Multi |                Single | BoundedWait |                   True |  2.298 ms | 0.0809 ms | 0.2387 ms |  2.295 ms | 435.11 |  0.03 |         - |         - |         - |    1800 B |
| ValueDisruptorPerf |                Multi |                Single | BoundedWait |                   True |  1.548 ms | 0.0432 ms | 0.1232 ms |  1.530 ms | 645.79 |  0.02 |         - |         - |         - |    1800 B |
