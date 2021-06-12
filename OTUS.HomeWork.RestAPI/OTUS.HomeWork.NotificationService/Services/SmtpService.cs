using Microsoft.Extensions.Options;
using OTUS.HomeWork.NotificationService.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OTUS.HomeWork.NotificationService.Services
{
    public class SmtpService
    {
        private readonly IOptions<SmtpOption> _smtpOption;
        public SmtpService(IOptions<SmtpOption> smtpOption)
        {
            _smtpOption = smtpOption;
        }

        public void SendEmail(string subject, string body, string destEmail)
        {
            var fromAddress = new MailAddress(_smtpOption.Value.SourceEmail, _smtpOption.Value.SourceEmailName);
            var toAddress = new MailAddress(destEmail, destEmail);
            var smtp = new SmtpClient
            {
                Host = _smtpOption.Value.Host,
                Port = _smtpOption.Value.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _smtpOption.Value.Password)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }
    }
}
