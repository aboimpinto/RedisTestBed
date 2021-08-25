using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisStreamGroupReader 
    {
        private readonly string _channel;
        private readonly string _group;
        private readonly string _consumer;

        public IDatabaseAsync ExecutorAsync  { get; private set; }

        public IDatabase Executor { get; set; }

        public RedisStreamGroupReader(string channel, string group, string consumer)
        {
            this._channel = channel;
            this._group = group;
            this._consumer = consumer;
        }

        // public async Task<RedisStreamItem<T>> GetNextMessage<T>()
        // {
        //     var message = await this.ExecutorAsync
        //         .StreamReadGroupAsync(this._channel, this._group, this._consumer, ">", count: 1)
        //         .ConfigureAwait(false);

        //     return message.Any() ? new RedisStreamItem<T>(message.First()) : null;
        // }

        public RedisStreamItem<T> GetNextMessage<T>()
        {
            var message = this.Executor
                .StreamReadGroup(this._channel, this._group, this._consumer, ">", count: 1);

            return message.Any() ? new RedisStreamItem<T>(message.First()) : null;
        }

        public IObservable<RedisStreamItem<T>> CreateObservableStream<T>(CancellationToken cancellationToken)
        {
            var scheduleInstance = ThreadPoolScheduler.Instance;

            return Observable.Create<RedisStreamItem<T>>(obs => 
            {
                var disposable = Observable
                    .Interval(TimeSpan.FromMilliseconds(200), scheduleInstance)
                    .Subscribe(async _ => 
                    {
                        var message = await this.ExecutorAsync
                            .StreamReadGroupAsync(
                                this._channel, 
                                this._group,
                                this._consumer, 
                                ">", 
                                count: 1)
                            .ConfigureAwait(false);

                        if (message.Any())
                        {
                            var redisStreamItem = new RedisStreamItem<T>(message.First()); 
                            obs.OnNext(redisStreamItem);
                        }
                        
                    });
                cancellationToken.Register(() => disposable.Dispose());

                return Disposable.Empty;
            });
        }

        public void SetDatabase(IDatabase database)
        {
            this.ExecutorAsync = database;
            this.Executor = database;
        }

        public void CreateStreamGroup()
        {
            try
            {
                this.Executor
                    .StreamCreateConsumerGroup(this._channel, this._group);
            }
            catch
            {
                // Group is already created
            }
        }
    }
}
