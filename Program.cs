using System;
using System.Threading;
using System.Threading.Tasks;
using RedisTestBed.Model;
using RedisTestBed.RedisProvider;

// https://www.codeproject.com/Articles/5269781/RedisProvider-for-NET
// https://stackoverflow.com/questions/40789943/observable-stream-from-stackexchange-redis-pub-sub-subscription

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
            var channelSubscriber1 = container
                .CreateChannelStreamer(channelName)
                .Subscribe(x => 
                {
                    Console.WriteLine($"#1 Notification Received: {x}");
                    Thread.Sleep(1000);
                });

            var channelSubscriber2 = container
                .CreateChannelStreamer(channelName)
                .Subscribe(x => 
                {
                    Console.WriteLine($"#2 Notification Received: {x}");
                    Thread.Sleep(500);
                });


            var messagesChannelPublisher = container.CreatePublisher(channelName);
            messagesChannelPublisher.Publish("Message: 1");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 2");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 3");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 4");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 5");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 6");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 7");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 8");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 9");
            Thread.Sleep(250);

            messagesChannelPublisher.Publish("Message: 10");
            Thread.Sleep(250);


            Console.ReadLine();
            channelSubscriber1.Dispose();
            channelSubscriber2.Dispose();
        }
    }
}
