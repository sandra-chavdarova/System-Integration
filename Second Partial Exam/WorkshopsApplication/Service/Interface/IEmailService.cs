using Domain.Dto.Email;

namespace Service.Interface;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
