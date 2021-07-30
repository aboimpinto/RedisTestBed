using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public static class SubscriberExtensions
    {
        public static IObservable<RedisValue> WhenNotified(this ISubscriber subscriber, RedisChannel channel)
        {
            return Observable.Create<RedisValue>(async (obs, ct) => 
            {
                var syncObs = Observer.Synchronize(obs);
                await subscriber
                    .SubscribeAsync(channel, (_, message) => 
                    {
                        syncObs.OnNext(message);
                    })
                    .ConfigureAwait(false);

                return Disposable.Create(() => subscriber.Unsubscribe(channel));
            });
        }
    }
}
