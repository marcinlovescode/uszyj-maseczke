using log4net;
using MailKit.Security;
using MimeKit;
using UszyjMaseczke.Domain.Message;
using UszyjMaseczke.WebApi.Configuration.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace UszyjMaseczke.Infrastructure.Mail
{
    public class MailService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MailService));

        private readonly MailConfiguration _mailConfiguration;

        public MailService(MailConfiguration mailConfiguration)
        {
            _mailConfiguration = mailConfiguration;
        }

        public void send(Message message)
        {
            var mimeMessage = new MimeMessage();
            
            mimeMessage.From.Add(new MailboxAddress(_mailConfiguration.getFrom.getDisplayName, _mailConfiguration.getFrom.getEmail));
            mimeMessage.To.Add(new MailboxAddress(message.Receiver));
            mimeMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Content;

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            send(mimeMessage);
        }

        private void send(MimeMessage message)
        {
            using (var client = new SmtpClient ())
            {
                client.Connect(_mailConfiguration.getHost, _mailConfiguration.getPort, SecureSocketOptions.SslOnConnect);

                client.Authenticate(_mailConfiguration.getFrom.getEmail, _mailConfiguration.getFrom.getPassword);

                client.Send(message);

                client.Disconnect(true);
            }
        }
    }
}