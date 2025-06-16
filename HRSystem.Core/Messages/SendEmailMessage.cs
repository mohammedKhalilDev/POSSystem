namespace POSSystem.Core.Messages
{
    public class SendEmailMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; } = "user";
        public string Email { get; set; } = "user@example.com";
    }
}
