using NotificationService.DTOs;

namespace NotificationService.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest request);
    }
}