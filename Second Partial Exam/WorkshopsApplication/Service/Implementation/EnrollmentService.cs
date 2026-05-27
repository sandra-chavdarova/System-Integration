using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EnrollmentService : IEnrollmentService
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IWorkshopService _workshopService;

    public EnrollmentService(IRepository<Enrollment> enrollmentRepository, IWorkshopService workshopService)
    {
        _enrollmentRepository = enrollmentRepository;
        _workshopService = workshopService;
    }

    public async Task<Enrollment> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Enrollment with ID: {id} not found");
        return result;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _enrollmentRepository.GetAsync(selector: x => x, predicate: x => x.Id == id);
    }

    public async Task<List<Enrollment>> GetAllAsync(string? dateAfter)
    {
        return await _enrollmentRepository.GetAllAsync(selector: x => x);
    }

    public async Task<Enrollment> CreateAsync(EnrollmentDto dto)
    {
        var enrollment = new Enrollment
        {
            Notes = dto.Notes,
            WorkshopId = dto.WorkshopId,
            VenueId = dto.VenueId,
            Status = EnrollmentStatus.Enrolled,
            UserId = dto.UserId
        };
        var result = await _enrollmentRepository.InsertAsync(enrollment);
        await _workshopService.IncrementParticipantsAsync(dto.WorkshopId);
        return await GetByIdNotNullAsync(result.Id);
    }

    public async Task<Enrollment> UpdateAsync(Guid id, EnrollmentDto dto)
    {
        var enrollment = await GetByIdNotNullAsync(id);
        enrollment.Notes = dto.Notes;
        enrollment.WorkshopId = dto.WorkshopId;
        enrollment.VenueId = dto.VenueId;
        enrollment.UserId = dto.UserId;
        return await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task<Enrollment> DeleteByIdAsync(Guid id)
    {
        var enrollment = await GetByIdNotNullAsync(id);
        var workshop = await _workshopService.GetByIdNotNullAsync(enrollment.WorkshopId);
        if (workshop.RegisteredParticipants > 0)
            throw new InvalidOperationException("Enrollments already registered for this workshop");
        if (workshop.StartTime <= DateTime.Now.AddHours(1))
            throw new InvalidOperationException("This workshop cannot be modified because it starts in less than 1 hour.");
        await _workshopService.DecrementParticipantsAsync(workshop.Id);
        await _enrollmentRepository.DeleteAsync(enrollment);
        return enrollment;
    }

    public async Task<PaginatedResult<Enrollment>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _enrollmentRepository.GetAllPagedAsync(selector: x => x, pageNumber: pageNumber, pageSize: pageSize);
    }

    public async Task<Enrollment> UpdateCancellationPathByIdAsync(Guid id, string path)
    {
        var enrollment = await GetByIdNotNullAsync(id);
        enrollment.CancellationDocumentPath = path;
        return await _enrollmentRepository.UpdateAsync(enrollment);
    }

    public async Task<List<Enrollment>> GetAllByWorkshopIdAsync(Guid workshopId)
    {
        return await _enrollmentRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.WorkshopId == workshopId,
            include: x => x.Include(a => a.User));
    }

    public async Task MarkAsAbsentByIdAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        result.Status = EnrollmentStatus.Absent;
        await _enrollmentRepository.UpdateAsync(result);
    }
}
