using System;
using StackExchange.Redis;

namespace RedisTestBed.RedisProvider
{
    public class RedisConnector
    {
        private Lazy<ConnectionMultiplexer> _lazyConnectionMultiplexer;
        private readonly int _databaseId;

        public IDatabase CurrentDatabase => this._lazyConnectionMultiplexer.Value.GetDatabase(this._databaseId);

        public RedisConnector(string configuration, int databaseId = 0)
        {
            this._lazyConnectionMultiplexer = new Lazy<ConnectionMultiplexer>(() => 
            {
                var connection = ConnectionMultiplexer.Connect(configuration);
                return connection;
            });
            this._databaseId = databaseId;
        }

        // public RedisConnector(ConnectionMultiplexer connection, int database = 0)
        // {
        //     this._db = connection.GetDatabase(database);
        // }

        // public async Task SetObjectAsync<T>(int key, T element)
        // {
        //     var stringKey = $"{element.GetType().Name}::{key.ToString()}";
        //     var elementRedisKey = new RedisKey(stringKey);

        //     var serializedElement = JsonSerializer.Serialize(element);
        //     var elementRedisValue = new RedisValue(serializedElement);

        //     await this._db
        //         .StringSetAsync(elementRedisKey, elementRedisValue)
        //         .ConfigureAwait(false);
        // }

        // public async Task<T> GetObjectAsync<T>(int key)
        // {
        //     var stringKey = $"{typeof(T).Name}::{key.ToString()}";
        //     var elementRedisKey = new RedisKey(stringKey);

        //     var result = await this._db
        //         .StringGetAsync(elementRedisKey);
            
        //     return JsonSerializer.Deserialize<T>(result);
        // }

        // public async Task AddToListAsync<T>(string listName, long key, T element)
        // {
        //     var listRedisKey = new RedisKey(listName);
        //     if (await this._db.KeyExistsAsync(listRedisKey))
        //     {
        //         // do nothing
        //     }
        //     else
        //     {
        //         await this._db.KeyPersistAsync(listRedisKey);
        //     }

        //     var exists = await this._db.KeyExistsAsync(listRedisKey);

        //     var serializedElement = JsonSerializer.Serialize(element);
        //     var elementRedisValue = new RedisValue(serializedElement);

        //     await this._db.ListSetByIndexAsync(listRedisKey, key, elementRedisValue);
        // }
    }
}