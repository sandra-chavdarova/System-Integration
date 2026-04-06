using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class AttendanceService : IAttendanceService
{
    private readonly IRepository<Attendance> _repository;
    private readonly IConsultationService _consultationService;

    public AttendanceService(IRepository<Attendance> repository, IConsultationService consultationService)
    {
        _repository = repository;
        _consultationService = consultationService;
    }

    public async Task<Attendance> GetByIdNotNullAsync(Guid id)
    {
        var result = await _repository.GetAsync(selector: x => x, predicate: x => x.Id.Equals(id));
        if (result == null)
        {
            throw new Exception("Attendance does not exist");
        }

        return result;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(selector: x => x, predicate: x => x.Id.Equals(id));
    }

    public async Task<List<Attendance>> GetAllAsync(string? dateAfter)
    {
        var result = await _repository.GetAllAsync(selector: x => x);
        return result.ToList();
    }

    public async Task<Attendance> CreateAsync(AttendanceDto dto)
    {
        var attendance = new Attendance()
        {
            Comment = dto.Comment,
            UserId = dto.UserId,
            RoomId = dto.RoomId,
            ConsultationId = dto.ConsultationId,
            Status = Status.Registered
        };
        var result = await _repository.InsertAsync(attendance);
        await _consultationService.IncrementRegisteredStudents(dto.ConsultationId);
        return result;
    }

    public async Task<Attendance> UpdateAsync(Guid id, AttendanceDto dto)
    {
        var attendance = await GetByIdNotNullAsync(id);
        attendance.Comment = dto.Comment;
        attendance.RoomId = dto.RoomId;
        attendance.ConsultationId = dto.ConsultationId;
        attendance.UserId = dto.UserId;
        return await _repository.UpdateAsync(attendance);
    }

    public async Task<Attendance> DeleteByIdAsync(Guid id)
    {
        var attendance = await GetByIdNotNullAsync(id);
        var consultation = await _consultationService.GetByIdNotNullAsync(attendance.ConsultationId);
        if (consultation.RegisteredStudents > 0)
        {
            throw new Exception("Attendances are already registered for this consultation");
        }

        if (consultation.StartTime <= DateTime.Now.AddHours(1))
        {
            throw new Exception("Consultation cannot be deleted");
        }

        await _consultationService.DecrementRegisteredStudents(consultation.Id);
        return await _repository.DeleteAsync(attendance);
    }

    public async Task<PaginatedResult<Attendance>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize
        );
    }

    public async Task<Attendance> UpdateReasonPathByIdAsync(Guid id, string path)
    {
        var attendance = await GetByIdNotNullAsync(id);
        attendance.CancellationReasonDocumentPath = path;
        return await _repository.UpdateAsync(attendance);
    }
    
    public async Task<List<Attendance>> GetAllByConsultationIdAsync(Guid consultationId)
    {
        return await _repository.GetAllAsync(
            selector: x => x,
            predicate: x => x.ConsultationId == consultationId,
            include: x => x.Include(a => a.User));
    }

    public async Task MarkAsAbsentByIdAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        result.Status = Status.Absent;
        await _repository.UpdateAsync(result);
    }
}