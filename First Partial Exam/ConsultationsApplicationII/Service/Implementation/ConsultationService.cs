using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ConsultationService : IConsultationService
{
    private readonly IRepository<Consultation> _repository;

    public ConsultationService(IRepository<Consultation> repository)
    {
        _repository = repository;
    }

    public async Task<Consultation> GetByIdNotNullAsync(Guid id)
    {
        var result = await _repository.GetAsync(selector: x => x, predicate: x => x.Id.Equals(id));
        if (result == null)
        {
            throw new InvalidOperationException("Consultation does not exist");
        }

        return result;
    }

    public async Task<Consultation?> GetByIdAsync(Guid id)
    {
        var result = await _repository.GetAsync(selector: x => x, predicate: x => x.Id.Equals(id));
        return result;
    }

    public async Task<List<Consultation>> GetAllAsync(string? roomName, DateOnly? date)
    {
        var result = await _repository.GetAllAsync(selector: x => x,
            predicate: x => (roomName == null || x.Room.Name.Contains(roomName)) &&
                            (date == null ? true : DateOnly.FromDateTime(x.StartTime).Equals(date)),
            include: x => x.Include(a => a.Attendances).ThenInclude(u => u.User));
        return result.ToList();
    }

    public async Task<Consultation> CreateAsync(DateTime startTime, DateTime endTime, Guid roomId)
    {
        var consultation = new Consultation()
        {
            StartTime = startTime,
            EndTime = endTime,
            RoomId = roomId,
            RegisteredStudents = 0
        };
        return await _repository.InsertAsync(consultation);
    }

    public async Task<Consultation> UpdateAsync(Guid id, DateTime startTime, DateTime endTime, Guid roomId)
    {
        var consultation = await GetByIdNotNullAsync(id);
        if (consultation.RegisteredStudents > 0)
        {
            throw new Exception("This consultation already has registered students");
        }

        consultation.StartTime = startTime;
        consultation.EndTime = endTime;
        consultation.RoomId = roomId;
        return await _repository.UpdateAsync(consultation);
    }

    public async Task<Consultation> DeleteByIdAsync(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        if (consultation.RegisteredStudents > 0)
        {
            throw new Exception("This consultation already has registered students");
        }

        return await _repository.DeleteAsync(consultation);
    }

    public async Task<PaginatedResult<Consultation>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(a => a.Attendances).ThenInclude(u => u.User),
            asNoTracking: true
        );
    }

    public async Task IncrementRegisteredStudents(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        consultation.RegisteredStudents += 1;
        await _repository.UpdateAsync(consultation);
    }

    public async Task DecrementRegisteredStudents(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        consultation.RegisteredStudents = Math.Max(0, consultation.RegisteredStudents - 1);
        await _repository.UpdateAsync(consultation);
    }
}