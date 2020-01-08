using System.Threading;
using System.Threading.Tasks;
using Disruptor.Dsl;

namespace ChannelsPlayground.Benchmark
{
    class DisruptorProducerFactory : DisruptorProducerFactoryBase
    {
        private readonly int _producerCount;
        private readonly int _itemsToProduce;
        private readonly Disruptor<ChannelsBenchmark.Event> _disruptor;
        private readonly TaskScheduler _scheduler;

        public DisruptorProducerFactory(int producerCount, int itemsToProduce, TaskScheduler scheduler, Disruptor<ChannelsBenchmark.Event> disruptor)
        {
            _producerCount = producerCount;
            _itemsToProduce = itemsToProduce;
            _scheduler = scheduler;
            _disruptor = disruptor;
        }

        public async Task StartProducersAsync(CancellationToken cancellationToken = default)
        {
            var producers = new Task[_producerCount];
            for (var producerIndex = 0; producerIndex < producers.Length; producerIndex++)
            {
                producers[producerIndex] = Task.Factory.StartNew(
                    () =>
                    {
                        var ringBuffer = _disruptor.RingBuffer;
                        for (var index = 0; index < _itemsToProduce; index++)
                        {
                            var seq = ringBuffer.Next();
                            var @event = ringBuffer[seq];
                            @event.Value = index;
                            ringBuffer.Publish(seq);
                        }
                    },
                    cancellationToken,
                    TaskCreationOptions.DenyChildAttach,
                    _scheduler
                );
            }

            await Task.WhenAll(producers);
            
            WaitForAllEventsToBeProcessed(_disruptor.RingBuffer);
        }
    }
}
