using ExamsApplication.Domain.Dto;
using ExamsApplication.Domain.Enum;
using ExamsApplication.Domain.Models;

namespace ExamsApplication.Service.Interface;

public interface IExamSlotService
{
    Task<ExamSlot> GetByIdNotNullAsync(Guid id);
    Task<List<ExamSlot>> GetAllAsync(string? sessionType);
    Task<ExamSlot> CreateAsync(ExamSlotDto dto);
    Task<ExamSlot> UpdateAsync(Guid id, ExamSlotDto dto);
    Task<ExamSlot> DeleteAsync(Guid id);
    Task<PaginatedResult<ExamSlot>> GetPagedAsync(int pageNumber, int pageSize);
}