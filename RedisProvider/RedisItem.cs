using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisItem<T> : RedisObject
    {
        public RedisItem(string keyName) : base(keyName)
        {
        }

        public Task<bool> Set(T value)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            return this.Executor.StringSetAsync(this.Key, new RedisValue(serializedValue));
        }

        public Task<T> Get()
        {   
            return this.Executor
                .StringGetAsync(this.Key)
                .ContinueWith(
                        x => JsonSerializer.Deserialize<T>(x.Result), 
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}