using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisIncrementalItem : RedisObject
    {
        public RedisIncrementalItem(string keyName) : base(keyName)
        {
        }

        public Task<long> Increment()
        {
            return this.Executor.StringIncrementAsync(this.Key);
        }

        public Task InitializeValue(long initialValue)
        {
            return this.Executor.StringSetAsync(this.Key, new RedisValue(initialValue.ToString()));
        }
    }
}
