using System.Threading;
using System.Threading.Tasks;
using Disruptor.Dsl;

namespace ChannelsPlayground.Benchmark
{
    class ValueDisruptorProducerFactory : DisruptorProducerFactoryBase
    {
        private readonly int _producerCount;
        private readonly int _itemsToProduce;
        private readonly ValueDisruptor<ChannelsBenchmark.ValueEvent> _disruptor;
        private readonly TaskScheduler _scheduler;
        private const int _batchSize = 20;

        public ValueDisruptorProducerFactory(int producerCount, int itemsToProduce, TaskScheduler scheduler, ValueDisruptor<ChannelsBenchmark.ValueEvent> disruptor)
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
                        for (var index = 0; index < _itemsToProduce;)
                        {
                            var hi = ringBuffer.Next(_batchSize);
                            var lo = hi - (_batchSize - 1);
                            for (var i = lo; i <= hi; i++)
                            { 
                                var @event = ringBuffer[i];
                                @event.Value = index;
                                index++;
                            }

                            ringBuffer.Publish(lo, hi);
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
