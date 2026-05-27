using Domain.Models;

namespace Service.Interface;

public interface IInboundEventEntryProcessor
{
    public Task ProcessPendingEventsAsync();
    public Task<Enrollment> ProcessEventEntry(InboundEventEntry entry);
}
