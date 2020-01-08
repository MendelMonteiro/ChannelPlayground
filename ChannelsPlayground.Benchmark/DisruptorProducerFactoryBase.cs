using System;
using Disruptor;

namespace ChannelsPlayground.Benchmark
{
    internal class DisruptorProducerFactoryBase
    {
        protected void WaitForAllEventsToBeProcessed(RingBuffer ringBuffer)
        {
            var timeoutAt = DateTime.UtcNow.AddSeconds(1);
            while (ringBuffer.GetRemainingCapacity() < ringBuffer.BufferSize)
            {
                if (DateTime.UtcNow > timeoutAt)
                    throw new ApplicationException();
                // Busy spin
            }
        }
    }
}
