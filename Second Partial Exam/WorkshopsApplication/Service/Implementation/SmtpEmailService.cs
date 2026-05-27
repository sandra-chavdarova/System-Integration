// TODO: Implement SmtpEmailService
// - Implement IEmailService
// - Inject IOptions<EmailSettings>, ILogger<SmtpEmailService>
// - Use MailKit to send emails:
//   1. Create MimeMessage with From, To, Subject
//   2. Build body with BodyBuilder (HtmlBody, PlainText, Attachments)
//   3. Connect via SmtpClient, Authenticate, Send
// - Handle attachments if present
// - Use settings: SmtpHost, SmtpPort, Username, Password, FromAddress, FromName, UseSsl

using Domain.Config;
using Domain.Dto.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.Interface;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Service.Implementation;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;


        var builder = new BodyBuilder()
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.PlainText
        };

        if (message.Attachments is { Count: > 0 })
        {
            foreach (var attachment in message.Attachments)
            {
               builder.Attachments.Add(attachment.FileName, attachment.Content,
                    ContentType.Parse(attachment.ContentType));
            }
        }


        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort,
                _settings.UseSsl
                    ? MailKit.Security.SecureSocketOptions.StartTls
                    : MailKit.Security.SecureSocketOptions.Auto, cancellationToken);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            throw new Exception();
        }
        finally
        {
            await smtp.DisconnectAsync(true, cancellationToken);
        }
    }
}