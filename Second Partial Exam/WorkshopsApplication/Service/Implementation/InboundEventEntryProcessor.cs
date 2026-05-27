// TODO: Implement InboundEventEntryProcessor
// - Implement IInboundEventEntryProcessor
// - Inject IEnrollmentService, IRepository<InboundEventEntry>
// - ProcessPendingEventsAsync: get pending entries (take 10), process each
// - ProcessEventEntry: deserialize RawPayload (PropertyNameCaseInsensitive!),
//   create Enrollment via service, mark as Completed/Failed

using System.Text.Json;
using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundEventEntryProcessor : IInboundEventEntryProcessor
{
    private readonly IRepository<InboundEventEntry> _repository;
    private readonly IEnrollmentService _enrollmentService;

    public InboundEventEntryProcessor(IRepository<InboundEventEntry> repository, IEnrollmentService enrollmentService)
    {
        _repository = repository;
        _enrollmentService = enrollmentService;
    }

    public async Task ProcessPendingEventsAsync()
    {
        var pending = await _repository.GetAllAsync(
            selector: x => x,
            predicate: e => e.Status == InboundEventStatus.Pending,
            orderBy: q => q.OrderBy(e => e.ReceivedAt),
            take: 10);
        foreach (var entry in pending)
        {
            await ProcessEventEntry(entry);
        }
    }

    public async Task<Enrollment> ProcessEventEntry(InboundEventEntry entry)
    {
        try
        {
            var request = JsonSerializer.Deserialize<EnrollmentRequestDto>(entry.RawPayload,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request == null)
            {
                throw new InvalidOperationException();
            }

            entry.Status = InboundEventStatus.Processing;
            await _repository.UpdateAsync(entry);

            var res = await _enrollmentService.CreateAsync(new EnrollmentDto()
            {
                Notes = request.Notes,
                UserId = request.UserId,
                VenueId = request.VenueId,
                WorkshopId = request.WorkshopId
            });

            entry.Status = InboundEventStatus.Completed;
            entry.ProcessedAt = DateTime.UtcNow;
            entry.EnrollmentId = res.Id;

            await _repository.UpdateAsync(entry);
            return res;
        }
        catch (Exception ex)
        {
            entry.Status = InboundEventStatus.Failed;
            entry.ErrorMessage = ex.Message;
            entry.ProcessedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entry);
            return null!;
        }
    }
}