using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelsPlayground.Benchmark
{
    public class Producer
    {
        private const int _batchSize = 10;
        private readonly int _itemsToProduce;
        private readonly ChannelWriter<int> _writer;

        public Producer(ChannelWriter<int> writer, int itemsToProduce)
        {
            _writer = writer;
            _itemsToProduce = itemsToProduce;
        }

        public async Task ProduceAsync(CancellationToken cancellationToken = default)
        {
            var itemsToProduce = _itemsToProduce;
            var writer = _writer;

            var i = 0;
            while (i < itemsToProduce)
            {
                await writer.WaitToWriteAsync(cancellationToken);
                for (var j = 0; j < _batchSize && i < itemsToProduce; j++)
                {
                    if (!writer.TryWrite(i))
                        break;

                    i++;
                }
            }
        }
    }
}
