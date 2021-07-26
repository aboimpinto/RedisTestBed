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
    }
}