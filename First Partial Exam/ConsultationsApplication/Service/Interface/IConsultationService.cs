using Domain.Dto;
using Domain.Models;

namespace Service.Interface;
public interface IConsultationService
{
    Task<Consultation> GetByIdNotNullAsync(Guid id);
    Task<List<Consultation>> GetAllAsync(string? dateAfter);
    Task<Consultation> CreateAsync(ConsultationDto dto);
    Task<Consultation> UpdateAsync(Guid id, ConsultationDto dto);
    Task<Consultation> DeleteByIdAsync(Guid id);
    Task<PaginatedResult<Consultation>> GetPagedAsync(int pageNumber, int pageSize);
}