using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POSSystem.Core.Entities;
using POSSystem.Core.Messages;
using POSSystem.Infrastructure.RabbitMQ.Configuration;
using POSSystem.Services.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSSystem.Services.Messaging.Enventory
{
    public class ItemStockConsumer : BackgroundService
    {
        private readonly RabbitMQConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly IServiceProvider _serviceProvider;

        public ItemStockConsumer(RabbitMQConfiguration rabbitMQConfig, IServiceProvider serviceProvider)
        {
            _configuration = rabbitMQConfig;
            _factory = new ConnectionFactory
            {
                HostName = _configuration.Server,
                UserName = _configuration.UserName,
                Password = _configuration.Password
            };

            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var connection = await _factory.CreateConnectionAsync();
                    using var channel = await connection.CreateChannelAsync();

                    var queue = _configuration.Queues["SaleItemDecreasingQueue"];

                    await channel.QueueDeclareAsync(
                        queue: queue.Name,
                        durable: queue.Durable,
                        exclusive: queue.Exclusive,
                        autoDelete: queue.AutoDelete,
                        arguments: null);

                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var itemSale = JsonSerializer.Deserialize<SaleItemMessage>(message);

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var itemsService = scope.ServiceProvider.GetRequiredService<IItemsService>();
                                await itemsService.DecreaseItemQuantity(itemSale.ItemId, itemSale.QuantitySold);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    };

                    await channel.BasicConsumeAsync(
                        queue: queue.Name,
                        autoAck: true,
                        consumer: consumer);

                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Consumer error: {ex.Message}");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
