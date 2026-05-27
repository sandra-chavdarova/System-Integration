// TODO: Implement BackgroundEnrollmentEntryService
// - Extend BackgroundService
// - Inject IServiceScopeFactory
// - In ExecuteAsync: loop every 1 minute, resolve IInboundEventEntryProcessor from scope, call ProcessPendingEventsAsync

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.Jobs;

public class BackgroundEnrollmentEntryService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundEnrollmentEntryService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IInboundEventEntryProcessor>();
            await service.ProcessPendingEventsAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}