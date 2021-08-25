using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisPublisher
    {
        private ConnectionMultiplexer _multiplexer;
        private string _channelName;

        public void SetConnectionMultiplexer(ConnectionMultiplexer multiplexer)
        {
            this._multiplexer = multiplexer;
        }

        public void SetChannelName(string channelName)
        {
            this._channelName = channelName;
        }

        public void Publish(string message)
        {
            var publisher = this._multiplexer.GetSubscriber();
            var x = publisher.Publish(this._channelName, message);
        }
    }
}
