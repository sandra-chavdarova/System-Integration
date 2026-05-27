using System.Threading.Channels;
using Domain.Dto.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Interface;

namespace Service.Jobs;

public class EmailBackgroundService:BackgroundService
{
    private readonly Channel<EmailMessage> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(Channel<EmailMessage> channel, IServiceScopeFactory scopeFactory,
        ILogger<EmailBackgroundService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider
                    .GetRequiredService<IEmailService>();
                await emailService.SendEmailAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send queued email to {To}", message.To);
                // In production: maybe implement retry with exponential backoff
            }
        }
    }
}