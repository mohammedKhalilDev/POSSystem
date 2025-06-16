using SendGrid.Helpers.Mail;

namespace POSSystem.Infrastructure.Email
{
    public class Emailmodel
    {
        public EmailAddress From { get; set; }
        public EmailAddress To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
