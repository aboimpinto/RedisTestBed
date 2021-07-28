using System;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace RedisTestBed.RedisProvider
{
    public class RedisSubscriber
    {
        private ConnectionMultiplexer _multiplexer;

        public void SetConnectionMultiplexer(ConnectionMultiplexer multiplexer)
        {
            this._multiplexer = multiplexer;
        }

        public void InitializeSubscriber(string channelName, Action<string> handler, PatternMode patternMode = RedisChannel.PatternMode.Literal)
        {
            var subscriber = this._multiplexer.GetSubscriber();
            
            subscriber.Subscribe(new RedisChannel(channelName, patternMode), (channel, message) => 
            {
                if (handler != null)
                {
                    handler(message);
                }
            });
        }
    }
}
