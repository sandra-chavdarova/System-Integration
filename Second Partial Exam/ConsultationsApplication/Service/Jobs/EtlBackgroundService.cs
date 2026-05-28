using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.Jobs;

public class EtlBackgroundService : BackgroundService
{
    private readonly IEtlSyncService _etlSyncService;
    private readonly IServiceScopeFactory _scopeFactory;

    public EtlBackgroundService(IEtlSyncService etlSyncService, IServiceScopeFactory scopeFactory)
    {
        _etlSyncService = etlSyncService;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var etlService = scope.ServiceProvider.GetRequiredService<IEtlSyncService>();
            await etlService.SyncAllAsync();

            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}