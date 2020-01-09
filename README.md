# ChannelPlayground

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18363
Intel Core i7-7700 CPU 3.60GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Job-ZEKMQI : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT

InvocationCount=1  UnrollFactor=1  

```
|             Method | PublisherCardinality | SubscriberCardinality |        Type | AllowSyncContinuations |     Mean |     Error |    StdDev |  Op/s | Ratio | RatioSD |
|------------------- |--------------------- |---------------------- |------------ |----------------------- |---------:|----------:|----------:|------:|------:|--------:|
|        **ChannelPerf** |                **Multi** |                **Single** | **BoundedWait** |                   **True** | **77.46 ms** | **1.5405 ms** | **2.8169 ms** | **12.91** |  **1.00** |    **0.00** |
|      DisruptorPerf |                Multi |                Single | BoundedWait |                   True | 31.48 ms | 0.6278 ms | 1.5518 ms | 31.77 |  0.41 |    0.03 |
| ValueDisruptorPerf |                Multi |                Single | BoundedWait |                   True | 33.83 ms | 0.6713 ms | 1.4593 ms | 29.56 |  0.44 |    0.02 |
|                    |                      |                       |             |                        |          |           |           |       |       |         |
|        **ChannelPerf** |               **Single** |                **Single** | **BoundedWait** |                   **True** | **76.19 ms** | **1.5541 ms** | **2.3261 ms** | **13.12** |  **1.00** |    **0.00** |
|      DisruptorPerf |               Single |                Single | BoundedWait |                   True | 10.34 ms | 0.3744 ms | 1.1039 ms | 96.75 |  0.13 |    0.02 |
| ValueDisruptorPerf |               Single |                Single | BoundedWait |                   True | 10.16 ms | 0.2770 ms | 0.8081 ms | 98.43 |  0.13 |    0.01 |
