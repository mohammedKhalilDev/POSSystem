using POSSystem.Core.Messages;
using POSSystem.Infrastructure.RabbitMQ.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Services.Messaging.ItemStock
{
    public interface IItemStockPublisher
    {
        Task Publish(SaleItemMessage email);
    }
    public class ItemStockPublisher : IItemStockPublisher
    {
        private readonly IMessagePublisher _messagePublisher;
        public ItemStockPublisher(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Publish(SaleItemMessage data)
        {
            _messagePublisher.Publish(data, "SaleItemDecreasingQueue");
        }
    }
}
