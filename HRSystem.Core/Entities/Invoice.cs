namespace POSSystem.Core.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }

        public virtual IEnumerable<InvoiceDetail> InvoiceDetails { get; set; }
    }

    public class InvoiceListDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceDetailedDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<InvoiceDetailItemDto> Items { get; set; }
    }
    public class InvoiceCreateDto
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public IEnumerable<InvoiceDetailCreateDto> Items { get; set; }
    }
    public class InvoiceUpdateDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public IEnumerable<InvoiceDetailUpdateDto> Items { get; set; }
    }

}
