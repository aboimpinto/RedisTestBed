using System;
using System.Threading.Tasks;
using RedisTestBed.Model;
using RedisTestBed.RedisProvider;

// https://www.codeproject.com/Articles/5269781/RedisProvider-for-NET

namespace RedisTestBed
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connector = new RedisConnector("127.0.0.1:6379,abortConnect=false");
            var container = new RedisContainer(connector, "TestBed");
            
            var incrementalUser = await container.CreateRedisIncrementalItem("incrementalUserId");
            var nextUserId = container.CreateRedisItem<long>("nextUserId");
            var nextProfileId = container.CreateRedisItem<long>("nextProfileId");
            
            var nextIncrementalUserId = await incrementalUser.Increment();
            Console.WriteLine(nextIncrementalUserId);

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

            var channelName = "messages";
            var channelSubscriber = container
                .CreateChannelStreamer(channelName)
                .Subscribe(x => 
                {
                    Console.WriteLine($"Notification Received: {x}");
                });

            var messagesChannelPublisher = container.CreatePublisher(channelName);
            messagesChannelPublisher.Publish("this is my message");


            Console.ReadLine();
            channelSubscriber.Dispose();
        }
    }
}
