using System;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisContainer
    {
        public IDatabase Database { get; private set; }

        public string KeyNameSpace { get; private set; }

        public RedisContainer(RedisConnector connector, string keyNameSpace = "")
        {
            this.Database = connector.CurrentDatabase;
            this.KeyNameSpace = keyNameSpace;
        }

        public RedisItem<T> CreateRedisItem<T>(string keyName)
        {
            var fullName = keyName;

            if (!string.IsNullOrEmpty(this.KeyNameSpace))
            {
                fullName = $"{this.KeyNameSpace}:{keyName}";
            }

            var instance = Activator.CreateInstance(typeof(RedisItem<T>), fullName) as RedisItem<T>;
            instance.SetDatabase(this.Database);
            return instance;
        }
    }
}