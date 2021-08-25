using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisStreamPublisher<T>
    {
        private readonly string _channel;

        public IDatabase Executor  { get; private set; }

        public RedisStreamPublisher(string channel)
        {
            this._channel = channel;
        }

        public async Task<string> Publish(T message)
        {
            var serializedValue = JsonSerializer.Serialize(message);
            var result = await this.Executor
                .StreamAddAsync(this._channel, "Message", serializedValue)
                .ConfigureAwait(false);
            
            return result;
        }

        public void SetDatabase(IDatabase database)
        {
            this.Executor = database;
        }
    }
}
