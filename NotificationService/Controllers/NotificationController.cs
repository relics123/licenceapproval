using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.DTOs;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public NotificationController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrWhiteSpace(request.RecipientEmail) || 
                    string.IsNullOrWhiteSpace(request.LicenseNumber))
                {
                    return BadRequest(new EmailResponse
                    {
                        Success = false,
                        Message = "Invalid email or license details"
                    });
                }

                // Generate email content based on notification type
                var (subject, body) = GenerateEmailContent(request);

                // Create notification record
                var notification = new Notification
                {
                    LicenseId = request.LicenseId,
                    LicenseNumber = request.LicenseNumber,
                    RecipientEmail = request.RecipientEmail,
                    RecipientName = request.RecipientName,
                    Subject = subject,
                    MessageBody = body,
                    NotificationType = request.NotificationType,
                    IsSent = false,
                    TenantId = request.TenantId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send email
                var emailSent = await _emailService.SendEmailAsync(request);

                if (emailSent)
                {
                    notification.IsSent = true;
                    notification.SentDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return Ok(new EmailResponse
                    {
                        Success = true,
                        Message = "Email notification sent successfully",
                        NotificationId = notification.Id
                    });
                }

                return StatusCode(500, new EmailResponse
                {
                    Success = false,
                    Message = "Failed to send email"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EmailResponse
                {
                    Success = false,
                    Message = $"Notification failed: {ex.Message}"
                });
            }
        }

       
        private (string subject, string body) GenerateEmailContent(EmailRequest request)
        {
            var subject = "";
            var body = "";

            switch (request.NotificationType.ToLower())
            {
                case "expiry":
                    var daysUntilExpiry = (request.ExpiryDate - DateTime.UtcNow).Days;
                    subject = $"License Expiry Notice - {request.LicenseNumber}";
                    body = $@"
Dear {request.RecipientName},

This is a reminder that your professional license is expiring soon.

License Number: {request.LicenseNumber}
Expiry Date: {request.ExpiryDate:dd-MMM-yyyy}
Days Remaining: {daysUntilExpiry} days

Please renew your license before the expiry date to avoid any service interruption.

Best regards,
License Management System
";
                    break;

                case "renewal":
                    subject = $"License Renewal Confirmation - {request.LicenseNumber}";
                    body = $@"
Dear {request.RecipientName},

Your license has been successfully renewed.

License Number: {request.LicenseNumber}
New Expiry Date: {request.ExpiryDate:dd-MMM-yyyy}

Thank you for renewing your license.

Best regards,
License Management System
";
                    break;

                case "approval":
                    subject = $"License Approved - {request.LicenseNumber}";
                    body = $@"
Dear {request.RecipientName},

Congratulations! Your license application has been approved.

License Number: {request.LicenseNumber}
Approval Date: {DateTime.UtcNow:dd-MMM-yyyy}
Expiry Date: {request.ExpiryDate:dd-MMM-yyyy}

You can now download your license certificate from the dashboard.

Best regards,
License Management System
";
                    break;

                default:
                    subject = $"License Notification - {request.LicenseNumber}";
                    body = $@"
Dear {request.RecipientName},

This is a notification regarding your license {request.LicenseNumber}.

Best regards,
License Management System
";
                    break;
            }

            return (subject, body);
        }
    }
}