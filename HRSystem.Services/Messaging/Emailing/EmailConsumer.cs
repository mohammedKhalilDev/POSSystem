using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POSSystem.Core.Messages;
using POSSystem.Infrastructure.Email;
using POSSystem.Infrastructure.RabbitMQ.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSSystem.Services.Messaging.Emailing
{
    public class EmailConsumer : BackgroundService
    {
        private readonly RabbitMQConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly IServiceProvider _serviceProvider;

        public EmailConsumer(RabbitMQConfiguration rabbitMQConfig, IServiceProvider serviceProvider)
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

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var queue = _configuration.Queues["EmailsToSendQueue"];

            channel.QueueDeclareAsync(queue: queue.Name, durable: queue.Durable, exclusive: queue.Exclusive, autoDelete: queue.AutoDelete, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var obj = JsonSerializer.Deserialize<SendEmailMessage>(message);

                Console.WriteLine($"Sending email to: {obj.Email}");


                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    service.SendEmail(obj).Wait();
                }

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue.Name, autoAck: true, consumer: consumer);

        }

    }

}
