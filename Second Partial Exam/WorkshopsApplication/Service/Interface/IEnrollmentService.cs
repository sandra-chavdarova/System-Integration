using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IEnrollmentService
{
    Task<Enrollment> GetByIdNotNullAsync(Guid id);
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<List<Enrollment>> GetAllAsync(string? dateAfter);
    Task<Enrollment> CreateAsync(EnrollmentDto dto);
    Task<Enrollment> UpdateAsync(Guid id, EnrollmentDto dto);
    Task<Enrollment> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Enrollment>> GetPagedAsync(int pageNumber, int pageSize);
    Task<Enrollment> UpdateCancellationPathByIdAsync(Guid id, string path);
    Task<List<Enrollment>> GetAllByWorkshopIdAsync(Guid workshopId);
    Task MarkAsAbsentByIdAsync(Guid id);
}
