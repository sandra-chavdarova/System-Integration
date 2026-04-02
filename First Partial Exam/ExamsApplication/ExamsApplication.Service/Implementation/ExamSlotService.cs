using ExamsApplication.Domain.Dto;
using ExamsApplication.Domain.Models;
using ExamsApplication.Repository.Interfaces;
using ExamsApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamsApplication.Service.Implementation;

public class ExamSlotService : IExamSlotService
{
    private readonly IRepository<ExamSlot> _repository;

    public ExamSlotService(IRepository<ExamSlot> repository)
    {
        _repository = repository;
    }

    public async Task<ExamSlot> GetByIdNotNullAsync(Guid id)
    {
        var result = await _repository.GetAsync(selector: x => x, predicate: x => x.Id.Equals(id));
        if (result == null)
        {
            throw new Exception("ExamSlot is null");
        }

        return result;
    }

    public async Task<List<ExamSlot>> GetAllAsync(string? sessionType)
    {
        if (sessionType == null)
        {
            throw new Exception("Session Type is null");
        }

        var result = await _repository.GetAllAsync(selector: x => x, predicate: x => x.SessionType.Equals(sessionType));
        return result.ToList();
    }

    public async Task<ExamSlot> CreateAsync(ExamSlotDto dto)
    {
        var examSlot = new ExamSlot()
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            SessionType = dto.SessionType,
            CourseId = dto.CourseId
        };
        return await _repository.InsertAsync(examSlot);
    }

    public async Task<ExamSlot> UpdateAsync(Guid id, ExamSlotDto dto)
    {
        var examSlot = await GetByIdNotNullAsync(id);
        examSlot.StartTime = dto.StartTime;
        examSlot.EndTime = dto.EndTime;
        examSlot.SessionType = dto.SessionType;
        examSlot.CourseId = dto.CourseId;
        return await _repository.UpdateAsync(examSlot);
    }

    public async Task<ExamSlot> DeleteAsync(Guid id)
    {
        var examSlot = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(examSlot);
    }

    public async Task<PaginatedResult<ExamSlot>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(y => y.ExamAttempts),
            asNoTracking: true
        );
    }
}