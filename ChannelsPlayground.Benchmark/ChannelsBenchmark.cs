using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using Disruptor;
using Disruptor.Dsl;

namespace ChannelsPlayground.Benchmark
{
    [Config(typeof(Config))]
    public class ChannelsBenchmark
    {
        private readonly EventProcessor _eventProcessor = new EventProcessor();
        private Disruptor<Event> _disruptorSingle;
        private ValueDisruptor<ValueEvent> _valueDisruptorSingle;
        private Disruptor<Event> _disruptorMulti;
        private ValueDisruptor<ValueEvent> _valueDisruptorMulti;

        public enum Cardinality
        {
            Single,
            Multi
        }

        public enum ChannelType
        {
            BoundedWait,
            Unbounded
        }

        private const int Single = 1;
        private const int Multi = 3;
        private const int Capacity = 1_000_000;
        private const int _ringBufferSize = 2 << 9;

        [Params(Cardinality.Multi, Cardinality.Single)]
        public Cardinality PublisherCardinality { get; set; }

        [Params( /*Cardinality.Multi, */Cardinality.Single)]
        public Cardinality SubscriberCardinality { get; set; }

        [Params(ChannelType.BoundedWait /*, ChannelType.Unbounded*/)]
        public ChannelType Type { get; set; }

        [Params(true /*, false*/)]
        public bool AllowSyncContinuations { get; set; }

        private FixedThreadPoolScheduler SubscriberScheduler { get; set; }
        private FixedThreadPoolScheduler PublisherScheduler { get; set; }

        private int SubscriberCount => SubscriberCardinality == Cardinality.Single ? Single : Multi;

        private int ProducerCount => PublisherCardinality == Cardinality.Single ? Single : Multi;

        [GlobalSetup]
        public void Setup()
        {
            SubscriberScheduler = new FixedThreadPoolScheduler(SubscriberCount);
            PublisherScheduler = new FixedThreadPoolScheduler(ProducerCount); 
        }

        [GlobalSetup(Target = nameof(DisruptorPerf))]
        public void SetupDisruptor()
        {
            Setup();
            _disruptorSingle = CreateDisruptor(ProducerType.Single, _ringBufferSize); 
            _disruptorMulti = CreateDisruptor(ProducerType.Multi, _ringBufferSize);
        }

        [GlobalSetup(Target = nameof(ValueDisruptorPerf))]
        public void SetupValueDisruptor()
        {
            Setup();
            _valueDisruptorSingle = CreateValueDisruptor(ProducerType.Single, _ringBufferSize);
            _valueDisruptorMulti = CreateValueDisruptor(ProducerType.Multi, _ringBufferSize);
        }

        private Disruptor<Event> CreateDisruptor(ProducerType producerType, int ringBufferSize)
        {
            var disruptor = new Disruptor<Event>(() => new Event(), ringBufferSize, TaskScheduler.Default, producerType, new BusySpinWaitStrategy());
            disruptor.HandleEventsWith(_eventProcessor);
            disruptor.Start();
            return disruptor;
        }

        private ValueDisruptor<ValueEvent> CreateValueDisruptor(ProducerType producerType, int ringBufferSize)
        {
            var valueDisruptor = new ValueDisruptor<ValueEvent>(() => new ValueEvent(), ringBufferSize, TaskScheduler.Default, producerType, new BusySpinWaitStrategy());
            valueDisruptor.HandleEventsWith(_eventProcessor);
            valueDisruptor.Start();
            return valueDisruptor;
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _eventProcessor.Count = 0;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            SubscriberScheduler.Dispose();
            PublisherScheduler.Dispose();
        }

        [GlobalCleanup(Target = nameof(DisruptorPerf))]
        public void CleanupDisruptor()
        {
            _disruptorSingle.Shutdown(TimeSpan.FromSeconds(1));
            _disruptorMulti.Shutdown(TimeSpan.FromSeconds(1));
            Cleanup();
        }

        [GlobalCleanup(Target = nameof(ValueDisruptorPerf))]
        public void CleanupValueDisruptor()
        {
            _valueDisruptorSingle.Shutdown(TimeSpan.FromSeconds(1));
            _valueDisruptorMulti.Shutdown(TimeSpan.FromSeconds(1));
            Cleanup();
        }

        [Benchmark(Baseline = true)]
        public async Task<int> ChannelPerf()
        {
            var channel = CreateChannel();
            var itemsToProduce = Capacity / ProducerCount;

            var producerFactory = new ProducerFactory(channel, ProducerCount, itemsToProduce, PublisherScheduler);
            var subscriberFactory = new SubscriberFactory(channel, SubscriberCount, SubscriberScheduler);

            var prodThread = producerFactory.StartProducersAsync();
            var subsThread = subscriberFactory.StartSubscribersAsync();

            await Task.WhenAll(prodThread, subsThread);

            return subsThread.Result;
        }

        [Benchmark]
        public int DisruptorPerf()
        {
            var disruptor = PublisherCardinality == Cardinality.Single ? _disruptorSingle : _disruptorMulti;
            var producer = new DisruptorProducerFactory(ProducerCount, Capacity / ProducerCount, PublisherScheduler, disruptor);
            producer.StartProducersAsync().Wait();
            return _eventProcessor.Count;
        }

        [Benchmark]
        public int ValueDisruptorPerf()
        {
            var disruptor = PublisherCardinality == Cardinality.Single ? _valueDisruptorSingle : _valueDisruptorMulti;
            var producer = new ValueDisruptorProducerFactory(ProducerCount, Capacity / ProducerCount, PublisherScheduler, disruptor);
            producer.StartProducersAsync().Wait();
            return _eventProcessor.Count;
        }

        public struct ValueEvent
        {
            public int Value { get; set; }
        }

        public class Event
        {
            public int Value { get; set; }
        }

        public class EventProcessor : IEventHandler<Event>, IValueEventHandler<ValueEvent>
        {
            public int Count { get; set; }
            public void OnEvent(Event data, long sequence, bool endOfBatch) => Count++;
            public void OnEvent(ref ValueEvent data, long sequence, bool endOfBatch) => Count++;
        }

        private Channel<int> CreateChannel()
        {
            switch (Type)
            {
                case ChannelType.BoundedWait:
                    return Channel.CreateBounded<int>(new BoundedChannelOptions(Capacity)
                    {
                        AllowSynchronousContinuations = AllowSyncContinuations,
                        FullMode = BoundedChannelFullMode.Wait,
                        SingleReader = SubscriberCardinality == Cardinality.Single,
                        SingleWriter = PublisherCardinality == Cardinality.Single
                    });
                case ChannelType.Unbounded:
                    return Channel.CreateUnbounded<int>(new UnboundedChannelOptions
                    {
                        AllowSynchronousContinuations = AllowSyncContinuations,
                        SingleReader = SubscriberCardinality == Cardinality.Single,
                        SingleWriter = PublisherCardinality == Cardinality.Single
                    });
                default:
                    throw new InvalidOperationException();
            }
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
