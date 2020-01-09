# ChannelPlayground

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18363
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Job-JBFMCV : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|             Method | PublisherCardinality | SubscriberCardinality |        Type | AllowSyncContinuations |      Mean |     Error |   StdDev |   Op/s | Ratio | RatioSD |
|------------------- |--------------------- |---------------------- |------------ |----------------------- |----------:|----------:|---------:|-------:|------:|--------:|
|        **ChannelPerf** |                **Multi** |                **Single** | **BoundedWait** |                   **True** | **58.535 ms** | **1.1468 ms** | **1.178 ms** |  **17.08** |  **1.00** |    **0.00** |
|      DisruptorPerf |                Multi |                Single | BoundedWait |                   True | 31.588 ms | 0.6319 ms | 1.687 ms |  31.66 |  0.55 |    0.03 |
| ValueDisruptorPerf |                Multi |                Single | BoundedWait |                   True | 30.814 ms | 0.6095 ms | 1.484 ms |  32.45 |  0.53 |    0.03 |
|                    |                      |                       |             |                        |           |           |          |        |       |         |
|        **ChannelPerf** |               **Single** |                **Single** | **BoundedWait** |                   **True** | **59.706 ms** | **1.1447 ms** | **1.272 ms** |  **16.75** |  **1.00** |    **0.00** |
|      DisruptorPerf |               Single |                Single | BoundedWait |                   True | 11.156 ms | 0.3927 ms | 1.139 ms |  89.64 |  0.17 |    0.02 |
| ValueDisruptorPerf |               Single |                Single | BoundedWait |                   True |  9.813 ms | 0.3530 ms | 1.030 ms | 101.90 |  0.15 |    0.02 |
