using POSSystem.Core.Messages;
using POSSystem.Infrastructure.RabbitMQ.Configuration;
using POSSystem.Infrastructure.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Services.Messaging.Emailing
{
    public interface IEmailPublisher
    {
        Task Publish(SendEmailMessage email);
    }
    public class EmailPublisher : IEmailPublisher
    {
        private readonly IMessagePublisher _messagePublisher;
        public EmailPublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Publish(SendEmailMessage email)
        {
            _messagePublisher.Publish(email, "EmailsToSendQueue");
        }


    }
}
