using BlogWeb.Application.Models.Emails;

namespace BlogWeb.Infrastructure.Services.Emails
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}