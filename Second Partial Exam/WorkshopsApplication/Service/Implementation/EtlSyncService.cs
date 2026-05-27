// TODO: Implement EtlSyncService
// - Implement IEtlSyncService
// - Inject IRepository<EtlSyncLog>, IWorkshopsRepository, IWorkshopsApiClient<ExternalWorkshopsDto>, IRepository<Venue>
// - SyncAllAsync:
//   1. Find last successful sync log with JobName "WorkshopsSync"
//   2. Call API with last sync date (or DateTime.MinValue)
//   3. Map ExternalWorkshopDto -> Workshop using GuidHelper.FromLegacyId("Workshop", externalId)
//   4. BulkInsertOrUpdate
//   5. Log success/failure to EtlSyncLog

using System.Reflection.Metadata;
using Domain.Config;
using Domain.Dto;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlSyncService : IEtlSyncService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IRepository<EtlSyncLog> _etlSyncLogRepository;
    private readonly IWorkshopsRepository _workshopsRepository;
    private readonly IWorkshopsApiClient<ExternalWorkshopsDto> _client;
    private readonly IRepository<Venue> _venueRepository;

    public EtlSyncService(IServiceScopeFactory serviceScopeFactory,
        IRepository<EtlSyncLog> etlSyncLogRepository, IWorkshopsRepository workshopsRepository,
        IWorkshopsApiClient<ExternalWorkshopsDto> client, IRepository<Venue> venueRepository)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _etlSyncLogRepository = etlSyncLogRepository;
        _workshopsRepository = workshopsRepository;
        _client = client;
        _venueRepository = venueRepository;
    }

    public async Task SyncAllAsync()
    {
        var syncLog = new EtlSyncLog
        {
            JobName = "WorkshopsSync",
            StartedAt = DateTime.UtcNow
        };

        try
        {
            var lastRun = await _etlSyncLogRepository.GetAllAsync(
                selector: x => x,
                predicate: x => x.JobName == "WorkshopsSync" && x.Success == true,
                orderBy: x => x.OrderByDescending(v => v.StartedAt));

            var date = lastRun.FirstOrDefault()?.StartedAt ?? DateTime.MinValue;

            var response = await _client.GetAllWorkshopsModifiedSinceAsync(date);
            var items = response.Items;
            var venues = await _venueRepository.GetAllAsync(x => x);

            var dictionary = venues.ToDictionary(x => x.Name, x => x.Id);

            var workshops = items.Select(x => new Workshop()
            {
                Id = GuidHelper.FromLegacyId("Workshop", x.ExternalId),
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                VenueId = dictionary[x.VenueName],
                Title = x.Title
            }).ToList();

            await _workshopsRepository.BulkInsertOrUpdateAsync(workshops);
            syncLog.Success = true;
        }
        catch (Exception ex)
        {
            syncLog.Success = false;
            syncLog.ErrorMessage = ex.Message;
        }
        finally
        {
            syncLog.CompletedAt = DateTime.UtcNow;
            await _etlSyncLogRepository.InsertAsync(syncLog);
        }
    }
}