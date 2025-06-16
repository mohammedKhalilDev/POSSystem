using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Infrastructure.RabbitMQ.Configuration
{
    public class RabbitMQConfiguration
    {
        public string Server { get; set; } = "";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public Dictionary<string, QueueConfig> Queues { get; set; }
    }

    public class QueueConfig
    {
        public string Name { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
    }
}
