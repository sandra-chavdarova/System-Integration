using Domain.Configuration;
using Domain.Dto;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlSyncService : IEtlSyncService
{
    private readonly IConsultationsRepository _repository;
    private readonly IRepository<EtlSyncLog> _etlSyncLogRepository;
    private readonly IConsultationsApiClient<ExternalConsultationsDto> _client;
    private readonly IRepository<Room> _roomRepository;

    public EtlSyncService(IConsultationsRepository repository, IRepository<EtlSyncLog> etlSyncLogRepository,
        IConsultationsApiClient<ExternalConsultationsDto> client, IRepository<Room> roomRepository)
    {
        _repository = repository;
        _etlSyncLogRepository = etlSyncLogRepository;
        _client = client;
        _roomRepository = roomRepository;
    }

    public async Task SyncAllAsync()
    {
        var syncLog = new EtlSyncLog
        {
            JobName = "ConsultationsSync",
            StartedAt = DateTime.UtcNow
        };

        try
        {
            var lastRun = await _etlSyncLogRepository.GetAllAsync(
                selector: x => x,
                predicate: x => x.JobName == "ConsultationsSync" && x.Success == true,
                orderBy: x => x.OrderByDescending(v => v.StartedAt));

            var date = lastRun.FirstOrDefault()?.StartedAt ?? DateTime.MinValue;

            var consultationsApi = await _client.GetAllConsultationsModifiedSinceAsync(date);
            var rooms = await _roomRepository.GetAllAsync(x => x);
            var items = consultationsApi.Items;

            var dict = rooms.ToDictionary(x => x.Name, x => x.Id);

            var consultations = items.Select(x => new Consultation()
            {
                Id = GuidHelper.FromLegacyId("Consultations", x.ExternalId),
                EndTime = x.EndTime,
                StartTime = x.StartTime,
                RoomId = dict[x.RoomName]
            }).ToList();

            await _repository.BulkInsertOrUpdateAsync(consultations);
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