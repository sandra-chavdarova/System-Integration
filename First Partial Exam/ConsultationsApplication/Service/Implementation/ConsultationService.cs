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
            throw new Exception("Consultation does not exist");
        }

        return result;
    }

    public async Task<List<Consultation>> GetAllAsync(string? dateAfter)
    {
        if (dateAfter == null)
        {
            throw new Exception("Date is null");
        }

        DateTime? dateTime = DateTime.Parse(dateAfter);
        var result =
            await _repository.GetAllAsync(selector: x => x, predicate: x => x.StartTime.Date >= dateTime.Value.Date);
        return result.ToList();
    }

    public async Task<Consultation> CreateAsync(ConsultationDto dto)
    {
        var consultation = new Consultation()
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            RoomId = dto.RoomId
        };
        return await _repository.InsertAsync(consultation);
    }

    public async Task<Consultation> UpdateAsync(Guid id, ConsultationDto dto)
    {
        var consultation = await GetByIdNotNullAsync(id);
        consultation.StartTime = dto.StartTime;
        consultation.EndTime = dto.EndTime;
        consultation.RoomId = dto.RoomId;
        return await _repository.UpdateAsync(consultation);
    }

    public async Task<Consultation> DeleteByIdAsync(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(consultation);
    }

    public async Task<PaginatedResult<Consultation>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(y => y.Attendances),
            asNoTracking: true
        );
    }
}