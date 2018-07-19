using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace StateMachines
{
    class Program
    {
        private static IConfiguration _config;

        static async Task Main(string[] args)
        {
            _config =
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = _config.GetAWSOptions();

            CancellationTokenSource cancelSource = new CancellationTokenSource();
            var cancel =  cancelSource.Token;
            SqsConsumer consumer = new SqsConsumer();

            Task.Run(() => consumer.Start(options, _config["QueueName"], cancel));

            /*using (var client = options.CreateServiceClient<IAmazonSimpleNotificationService>())
            {
                Console.WriteLine("Creating orders");

                using (TextReader reader = new StreamReader(File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "Orders.txt"))))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(",");
                        await PublishOrder(client, new Order(int.Parse(parts[1]), parts[0]));
                    }
                }

                Console.WriteLine("Orders created");
            }*/

            Console.WriteLine("Done");
            Console.ReadLine();
            cancelSource.Cancel();
        }

        private static async Task PublishOrder(IAmazonSimpleNotificationService snsClient, Order order)
        {
            await snsClient.PublishAsync(
                _config["OrderTopic"],
                JsonConvert.SerializeObject(order)
            );
        }
    }
}
