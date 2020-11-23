using BenchmarkDotNet.Running;

namespace ChannelsPlayground.Benchmark
{
    internal static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<ChannelsBenchmark>();
            //RunDisruptor();
        }

        private static void RunDisruptor()
        {
            var channelsBenchmark = new ChannelsBenchmark();

            channelsBenchmark.SetupDisruptor();

            channelsBenchmark.IterationSetup();
            channelsBenchmark.DisruptorPerf();

            //channelsBenchmark.IterationSetup();
            //channelsBenchmark.DisruptorPerf();

            channelsBenchmark.Cleanup();
            channelsBenchmark.CleanupDisruptor();
        }
    }
}