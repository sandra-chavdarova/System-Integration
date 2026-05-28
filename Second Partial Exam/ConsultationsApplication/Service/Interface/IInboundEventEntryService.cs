namespace Service.Implementation;

public interface IInboundEventEntryService
{
    Task<InboundEventEntry> CreateAsync(string rawPayload);
    Task<InboundEventEntry> GetByIdNotNull(Guid id);
}