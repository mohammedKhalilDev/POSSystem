namespace POSSystem.Core.Entities
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public virtual Item Item { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
    public class InvoiceDetailItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } // Assuming Item has a Name property
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Assuming Item has a Price property
        public decimal Total { get; set; }
    }
    public class InvoiceDetailCreateDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
    public class InvoiceDetailUpdateDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
