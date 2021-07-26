using System;
using System.Threading.Tasks;
using RedisTestBed.Model;
using RedisTestBed.RedisProvider;
using StackExchange.Redis;

// https://www.codeproject.com/Articles/5269781/RedisProvider-for-NET

namespace RedisTestBed
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var redis = ConnectionMultiplexer.Connect(
            //     new ConfigurationOptions{
            //         EndPoints = {"localhost:6379"}                
            //     });

            var connector = new RedisConnector("127.0.0.1:6379,abortConnect=false");
            var container = new RedisContainer(connector, "TestBed");
            
            var nextUserId = container.CreateRedisItem<long>("nextUserId");
            var nextProfileId = container.CreateRedisItem<long>("nextProfileId");
            
            await nextUserId.Set(28);
            await nextProfileId.Set(1);

            var user1 = new User
            {
                Id = 1,
                FirstName = "Paulo",
                LastName = "Pinto"
            };
            var user = container.CreateRedisItem<User>("Users");
            await user.Set(user1);

            // var value = await nextUserId.Get();

            // var db = redis.GetDatabase();
            // var pong = await db.PingAsync();
            // Console.WriteLine(pong);

            

            // await db.StringSetAsync(new RedisKey("User:PauloAboimPinto:Name"), new RedisValue("Paulo Aboim Pinto"));
            // await db.StringSetAsync(new RedisKey("User:PauloAboimPinto:DateOfBirth"), new RedisValue("28.08.1974"));

            // var result = await db.StringGetAsync(new RedisKey("User:PauloAboimPinto:Name"));
            // Console.WriteLine(result);

            // var user1 = new User
            // {
            //     Id = 1,
            //     FirstName = "Paulo",
            //     LastName = "Pinto"
            // };
            // // await connector.SetObjectAsync<User>(1, user1);
            // await connector.AddToListAsync<User>("Users", 1, user1);

            // var user2 = new User
            // {
            //     Id = 2,
            //     FirstName = "Sophia",
            //     LastName = "Walker"
            // };
            // // await connector.SetObjectAsync<User>(2, user2);
            // await connector.AddToListAsync<User>("Users", 2, user2);

            // // var result = await connector.GetObjectAsync<User>(2);

        }
    }

    
}
