using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public abstract class RedisObject
    {
        public IDatabaseAsync Executor  { get; private set; }

        public RedisKey Key { get; private set; }

        public RedisObject(string keyName)
        {
            this.Key = new RedisKey(keyName);
        }

        public void SetDatabase(IDatabase database)
        {
            this.Executor = database;
        }

        public Task<bool> Exists()
        {
            return this.Executor.KeyExistsAsync(this.Key);
        }
    }
}