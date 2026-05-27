using Domain.Models;

namespace Service.Interface;

public interface IInboundEventEntryService
{
    Task<InboundEventEntry> CreateAsync(string rawPayload);
    Task<InboundEventEntry> GetByIdNotNullAsync(Guid id);
}
