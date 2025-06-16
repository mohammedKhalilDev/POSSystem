using Microsoft.AspNetCore.Connections;
using POSSystem.Infrastructure.RabbitMQ.Configuration;
using POSSystem.Infrastructure.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSSystem.Infrastructure.RabbitMQ.Service
{
    public class RabbitMQPublisher : IMessagePublisher
    {
        private readonly RabbitMQConfiguration _configuration;
        private readonly ConnectionFactory _factory;

        public RabbitMQPublisher(RabbitMQConfiguration rabbitMQConfig)
        {
            _configuration = rabbitMQConfig;
            _factory = new ConnectionFactory
            {
                HostName = _configuration.Server,
                UserName = _configuration.UserName,
                Password = _configuration.Password
            };
        }
        public async Task Publish<T>(T message, string queueName)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var queue = _configuration.Queues[queueName];

            await channel.QueueDeclareAsync(
                queue: queue.Name,
                durable: queue.Durable,
                exclusive: queue.Exclusive,
                autoDelete: queue.AutoDelete,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);

        }

        //public void Dispose()
        //{
        //    _channel?.Close();
        //    _channel?.Dispose();
        //    _connection?.Close();
        //    _connection?.Dispose();
        //}
    }
}
