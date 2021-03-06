using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace RedisTestBed.RedisProvider
{
    public class RedisContainer
    {
        private IDictionary<string, RedisObject> _trackedObjects;

        private ConnectionMultiplexer _multiplexer;

        public IDatabase Database { get; private set; }

        public string KeyNameSpace { get; private set; }

        public RedisContainer(RedisConnector connector, string keyNameSpace = "")
        {
            this.Database = connector.CurrentDatabase;
            this._multiplexer = connector.ConnectionMultiplexer;
            this.KeyNameSpace = keyNameSpace;

            this._trackedObjects = new Dictionary<string, RedisObject>();
        }

        public RedisItem<T> CreateRedisItem<T>(string keyName)
        {
            var fullName = this.CalculateFullKeyName(keyName);

            RedisObject obj;
            if (this._trackedObjects.TryGetValue(fullName, out obj))
            {
                return obj as RedisItem<T>;
            }

            var instance = Activator.CreateInstance(typeof(RedisItem<T>), fullName) as RedisItem<T>;
            instance.SetDatabase(this.Database);

            this._trackedObjects.Add(fullName, instance);
            return instance;
        }

        public async Task<RedisIncrementalItem> CreateRedisIncrementalItem(string keyName, long initialValue = 0)
        {
            var fullName = this.CalculateFullKeyName(keyName);

            RedisObject obj;
            if (this._trackedObjects.TryGetValue(fullName, out obj))
            {
                return obj as RedisIncrementalItem;
            }

            var instance = Activator.CreateInstance(typeof(RedisIncrementalItem), fullName) as RedisIncrementalItem;
            instance.SetDatabase(this.Database);

            if (!await instance.Exists())
            {
                await instance.InitializeValue(initialValue);
            }

            this._trackedObjects.Add(fullName, instance);
            return instance;
        }

        public RedisSubscriber CreateSubscriber(string channelName)
        {
            var instance = Activator.CreateInstance(typeof(RedisSubscriber), this._multiplexer, channelName) as RedisSubscriber;
            return instance;
        }

        public RedisPublisher CreatePublisher(string channelName)
        {
            var instance = Activator.CreateInstance(typeof(RedisPublisher)) as RedisPublisher;
            instance.SetConnectionMultiplexer(this._multiplexer);
            instance.SetChannelName(channelName);

            return instance;
        }

        // public async Task<RedisStreamGroupReader> CreateSteamGroupReader(string channelName, string group, string consumer)
        // {
        //     var instance = Activator.CreateInstance(typeof(RedisStreamGroupReader), channelName, group, consumer) as RedisStreamGroupReader;
        //     instance.SetDatabase(this.Database);
        //     await instance.CreateStreamGroup().ConfigureAwait(false);

        //     return instance;
        // }

        // public RedisStreamPublisher<T> CreateStreamPublisher<T>(string channelName)
        // {
        //     var instance = Activator.CreateInstance(typeof(RedisStreamPublisher<T>), channelName) as RedisStreamPublisher<T>;
        //     instance.SetDatabase(this.Database);

        //     return instance;
        // }

        public RedisStreamGroupReader CreateSteamGroupReader(string channelName, string group, string consumer)
        {
            var instance = Activator.CreateInstance(typeof(RedisStreamGroupReader), channelName, group, consumer) as RedisStreamGroupReader;
            instance.SetDatabase(this.Database);
            instance.CreateStreamGroup();

            return instance;
        }

        public RedisStreamPublisher<T> CreateStreamPublisher<T>(string channelName)
        {
            var instance = Activator.CreateInstance(typeof(RedisStreamPublisher<T>), channelName) as RedisStreamPublisher<T>;
            instance.SetDatabase(this.Database);

            return instance;
        }

        private string CalculateFullKeyName(string keyName)
        {
            var fullName = keyName;

            if (!string.IsNullOrEmpty(this.KeyNameSpace))
            {
                fullName = $"{this.KeyNameSpace}:{keyName}";
            }

            return fullName;
        }
    }
}