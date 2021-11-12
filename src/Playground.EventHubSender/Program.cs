using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Playground.EventHubSender
{
    class Program
    {
        static readonly Guid s_tenantId = Guid.NewGuid();
        static readonly Random s_random = new(123456);
        static readonly List<string> s_contractIds = new();

        const string EventHubNamespace = "euan-evn";

        const string ContractsEventHubName = "np-contracts";
        const int ContractsWriteCount = 10;
        const int ContractsSleepTime = 60 * 1000;

        const string OrdersEventHubName = "np-orders";
        const int OrdersWriteCount = 10;
        const int OrdersSleepTime = 2000;

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            IConfiguration configuration = host.Services.GetService<IConfiguration>();
            string connectionString = configuration.GetConnectionString(EventHubNamespace);

            var contractProducerTask = ProduceContracts(connectionString);
            var ordersProducerTask = ProduceOrders(connectionString);

            await host.RunAsync();
        }

        private static async ValueTask ProduceContracts(string connectionString)
        {
            await using (var producerClient = new EventHubProducerClient(connectionString, ContractsEventHubName))
            {
                var runCount = 0;
                while (true)
                {
                    for (int i = 0; i < ContractsWriteCount; ++i)
                    {
                        string contractId = $"C_{i}";
                        s_contractIds.Add(contractId);
                        var eventData = new EventData(Encoding.UTF8.GetBytes(GenerateContract(contractId)));
                        eventData.Properties["tenantId"] = s_tenantId;
                        await producerClient.SendAsync(new List<EventData> { eventData }, new SendEventOptions { PartitionKey = contractId });
                    }
                    Console.WriteLine($"Run {runCount} - Output {ContractsWriteCount} contracts");
                    await Task.Delay(ContractsSleepTime);
                    ++runCount;
                }
            }
        }

        private static async ValueTask ProduceOrders(string connectionString)
        {
            await using (var producerClient = new EventHubProducerClient(connectionString, OrdersEventHubName))
            {
                var runCount = 0;
                while (true)
                {
                    await Task.Delay(OrdersSleepTime);
                    for (int i = 0; i < OrdersWriteCount; ++i)
                    {
                        string orderId = $"O_{runCount}_{i}";
                        var eventData = new EventData(Encoding.UTF8.GetBytes(GenerateOrder(orderId)));
                        eventData.Properties["tenantId"] = s_tenantId;
                        await producerClient.SendAsync(new List<EventData> { eventData }, new SendEventOptions { PartitionKey = orderId });
                    }
                    Console.WriteLine($"Run {runCount} - Output {OrdersWriteCount} orders");
                    ++runCount;
                }
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment(Environments.Development);

        private static string GenerateContract(string contractId)
            => $"{{" +
            $"\"marketId\":\"NordPool\"," +
            $"\"contractId\":\"{contractId}\"," +
            $"\"updatedAt\":\"{DateTimeOffset.UtcNow:yyyy-MM-ddTHH:mm:ss.ffffffZ}\"," +
            $"\"state\":\"{(s_random.NextDouble() > 0.5 ? "ACTI" : "IACT")}\"" +
            $"}}";

        private static string GenerateOrder(string orderId)
            => $"{{\"contractId\":\"{s_contractIds[s_random.Next(s_contractIds.Count)]}\",\"orderId\":\"{orderId}\",\"quantity\":{s_random.Next(1000, 10000)},\"createdAt\":\"{DateTimeOffset.UtcNow:yyyy-MM-ddTHH:mm:ss.ffffffZ}\"}}";
    }
}
