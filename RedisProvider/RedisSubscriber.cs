using System;
using System.Reactive.Linq;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisSubscriber : IObservable<RedisValue>
    {
        private IObservable<RedisValue> _channelStream;

        public RedisSubscriber(ConnectionMultiplexer multiplexer, string channelName)
        {
            this._channelStream = multiplexer
                .GetSubscriber()
                .WhenNotified(channelName);
        }

        public IDisposable Subscribe(IObserver<RedisValue> observer)
        {
            return this._channelStream
                .Subscribe(message => observer.OnNext(message));
        }
    }
}
