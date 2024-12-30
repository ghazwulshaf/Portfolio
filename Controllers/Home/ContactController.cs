using GhazwulShaf.Models;
using GhazwulShaf.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace GhazwulShaf.Controllers.Home
{
    [Route("/contact")]
    public class ContactController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ContactService _contactService;

        public ContactController(IConfiguration configuration, ContactService contactService)
        {
            _configuration = configuration;
            _contactService = contactService;
        }

        // GET: View Home Contact Page
        [HttpGet, Route("")]
        public async Task<IActionResult> Index()
        {
            var contact = await _contactService.GetAsync();
            return View("/Views/Home/Contact/Index.cshtml", contact);
        }

        // POST: Send Message to Email
        [HttpPost, Route("send-message")]
        public async Task<IActionResult> SendMessage(string name, string email, string subject, string message)
        {
            if (name != null && email != null && subject != null && message != null)
            {
                try
                {
                    var emailService = new MimeMessage();
                    emailService.From.Add(new MailboxAddress(name, email));
                    emailService.To.Add(new MailboxAddress("Admin", _configuration["EmailSettings:RecipientEmail"]));
                    emailService.Subject = subject;
                    emailService.Body = new TextPart("plain");

                    using var smtp = new SmtpClient();
                    smtp.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]!), true);
                    smtp.Send(emailService);
                    smtp.Disconnect(true);

                    ViewData["Send_Success"] = "Message sent successfully";
                }
                catch (Exception ex)
                {
                    ViewData["Send_Error"] = $"Failed to send message: {ex.Message}";
                }
            }

            var contact = await _contactService.GetAsync();
            return View("/Views/Home/Contact/Index.cshtml", contact);
        }
    }
}
