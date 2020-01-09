# ChannelPlayground

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18363
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Job-YZOVLR : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|             Method | PublisherCardinality | SubscriberCardinality |        Type | AllowSyncContinuations |     Mean |     Error |    StdDev |  Op/s | Ratio | RatioSD |
|------------------- |--------------------- |---------------------- |------------ |----------------------- |---------:|----------:|----------:|------:|------:|--------:|
|        **ChannelPerf** |                **Multi** |                **Single** | **BoundedWait** |                   **True** | **60.55 ms** | **1.1341 ms** | **0.9470 ms** | **16.51** |  **1.00** |    **0.00** |
|      DisruptorPerf |                Multi |                Single | BoundedWait |                   True | 32.80 ms | 0.6521 ms | 1.7178 ms | 30.49 |  0.55 |    0.03 |
| ValueDisruptorPerf |                Multi |                Single | BoundedWait |                   True | 30.64 ms | 0.6067 ms | 1.5769 ms | 32.63 |  0.51 |    0.03 |
|                    |                      |                       |             |                        |          |           |           |       |       |         |
|        **ChannelPerf** |               **Single** |                **Single** | **BoundedWait** |                   **True** | **63.66 ms** | **1.2459 ms** | **1.9026 ms** | **15.71** |  **1.00** |    **0.00** |
|      DisruptorPerf |               Single |                Single | BoundedWait |                   True | 11.05 ms | 0.3077 ms | 0.8926 ms | 90.48 |  0.17 |    0.02 |
| ValueDisruptorPerf |               Single |                Single | BoundedWait |                   True | 10.17 ms | 0.3276 ms | 0.9607 ms | 98.30 |  0.15 |    0.02 |


