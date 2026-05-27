// TODO: Implement SyncWorkshopsBackgroundService
// - Extend BackgroundService
// - Inject IServiceScopeFactory
// - In ExecuteAsync: loop every 5 minutes, resolve IEtlSyncService from scope, call SyncAllAsync

using Domain.Dto;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Interface;

namespace Service.Jobs;

public class SyncWorkshopsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SyncWorkshopsBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEtlSyncService>();
            await service.SyncAllAsync();
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}