using Microsoft.AspNetCore.Identity.UI.Services;

namespace BooksSpring2024
{
    //any class that has an I in front of it is an interface, a template
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
