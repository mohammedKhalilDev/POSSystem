using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Core.Messages
{
    public class SaleItemMessage
    {
        public int ItemId { get; set; }
        public int QuantitySold { get; set; }
    }
}
