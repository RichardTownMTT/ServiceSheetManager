using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ServiceSheetManager.Helpers
{
    public class SendGridEmail
    {
        public static async Task SendEmail(string message)
        {
            var email = new SendGridMessage();
            email.SetFrom(new EmailAddress("r.town@mtt.uk.com", "MTT"));
            email.AddTo("r.town@mtt.uk.com", "Richard");
            email.AddTo("c.oconnor@mtt.uk.com", "Callum");
            email.SetSubject("Equipment Tracker - Barcode not recognised");
            email.AddContent(MimeType.Text, message);


            var apiKey = ConfigurationManager.AppSettings["sendgridApiKey"];
            var client = new SendGridClient(apiKey);
            var response = await client.SendEmailAsync(email);
        }
    }
}