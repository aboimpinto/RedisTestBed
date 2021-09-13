using System;
using System.Linq;
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
            
            // await RedisItemExample(container);

            // await IncrementalItemExample(container);
            
            await StreamExample(container).ConfigureAwait(false);

            Console.ReadLine();
        }

        private static async Task RedisItemExample(RedisContainer container)
        {
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
        }

        private static async Task IncrementalItemExample(RedisContainer container)
        {
            var incrementalUser = await container.CreateRedisIncrementalItem("incrementalUserId");

            var nextIncrementalUserId = await incrementalUser.Increment();
            Console.WriteLine(nextIncrementalUserId);
        }

        private static async Task StreamExample(RedisContainer container)
        {
            var streamChannel = "myChannel";
            var streamGroup = "ChannelGroup1";
            var consumer1 = "ChannelConsumer1";
            var consumer2 = "ChannelConsumer2";

            var cancellationToken = new CancellationTokenSource();
            var myChannelStreamConsumer1 = container.CreateSteamGroupReader(streamChannel, streamGroup, consumer1);
            var myChannelStreamConsumer2 = container.CreateSteamGroupReader(streamChannel, streamGroup, consumer2);
            

            var streamPublisher = container.CreateStreamPublisher<string>(streamChannel);
            Console.WriteLine(await streamPublisher.Publish("Message1").ConfigureAwait(false));
            Console.WriteLine(await streamPublisher.Publish("Message2").ConfigureAwait(false));
            Console.WriteLine(await streamPublisher.Publish("Message3").ConfigureAwait(false));

            var streamGroupReaderSubscriber1 = myChannelStreamConsumer1
                .CreateObservableStream<string>(cancellationToken.Token)
                .Subscribe(x => 
                {
                    Console.WriteLine($"--> {consumer1} {x.Id}::{x.Message}");
                    Thread.Sleep(3000);
                });

            var streamGroupReaderSubscriber2 = myChannelStreamConsumer2
                .CreateObservableStream<string>(cancellationToken.Token)
                .Subscribe(x => 
                {
                    Console.WriteLine($"--> {consumer2} {x.Id}::{x.Message}");
                    Thread.Sleep(4000);
                });
        }
    }
}
