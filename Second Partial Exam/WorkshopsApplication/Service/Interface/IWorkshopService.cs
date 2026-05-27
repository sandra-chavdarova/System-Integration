using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IWorkshopService
{
    Task<Workshop> GetByIdNotNullAsync(Guid id);
    Task<Workshop?> GetByIdAsync(Guid id);
    Task<List<Workshop>> GetAllAsync(string? venueName, DateOnly? date);
    Task<Workshop> CreateAsync(string title, DateTime startTime, DateTime endTime, Guid venueId);
    Task<Workshop> UpdateAsync(Guid id, string title, DateTime startTime, DateTime endTime, Guid venueId);
    Task<Workshop> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Workshop>> GetPagedAsync(int pageNumber, int pageSize);
    Task IncrementParticipantsAsync(Guid id);
    Task DecrementParticipantsAsync(Guid id);
}
