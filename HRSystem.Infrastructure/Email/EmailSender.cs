using POSSystem.Core.Messages;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSSystem.Infrastructure.Email
{
    public interface IEmailSender
    {
        Task SendEmail(SendEmailMessage emailContent);

    }
    public class EmailSender : IEmailSender
    {

        public async Task SendEmail(SendEmailMessage emailContent)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var email = await BuildEmail(emailContent);
            var msg = MailHelper.CreateSingleEmail(email.From, email.To, email.Subject, email.Body, null);
            await client.SendEmailAsync(msg);
        }

        private async Task<Emailmodel> BuildEmail(SendEmailMessage emailContent)
        {
            var email = new Emailmodel();
            email.From = new EmailAddress("noreply@example.com", "ASP.NET Core");
            email.To = new EmailAddress(emailContent.Email, emailContent.Name);
            email.Subject = "New Order Created";
            email.Body = $"Dear: {emailContent.Name} your Order: {emailContent.OrderId} created successfully";

            return email;
        }
        private async Task<string> BuildEmailBody(SendEmailMessage emailContent)
        {
            return $"Dear: {emailContent.Name} your Order: {emailContent.OrderId} created successfully";
        }

    }
}
