using Domain.Dto.Email;

namespace Service.Interface;

public interface IEmailQueue
{
    Task EnqueueAsync(EmailMessage message);
}
