using Domain.Config;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WorkshopService : IWorkshopService
{
    private readonly IRepository<Workshop> _repository;
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _cacheSettings;

    public WorkshopService(IRepository<Workshop> repository, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
    {
        _repository = repository;
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<Workshop> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (result == null)
            throw new InvalidOperationException($"Workshop with id {id} not found");
        return result;
    }

    public async Task<Workshop?> GetByIdAsync(Guid id)
    {
        return await _repository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    // TODO: cache
    public async Task<List<Workshop>> GetAllAsync(string? venueName, DateOnly? date)
    {
        var cacheKey = $"workshops:{venueName}:{date:yyyy-MM-dd}";
        if (_cache.TryGetValue(cacheKey, out List<Workshop>? cached))
        {
            return cached;
        }

        if (cached != null)
        {
            return cached;
        }

        var apiData = await _repository.GetAllAsync(
            selector: x => x,
            predicate: x =>
                (venueName == null || x.Venue.Name.Contains(venueName)) &&
                (date == null || DateOnly.FromDateTime(x.StartTime) == date),
            include: x => x.Include(w => w.Enrollments).ThenInclude(e => e.User));

        _cache.Set(cacheKey, apiData, TimeSpan.FromMinutes(_cacheSettings.ListCacheDurationMinutes));

        return apiData;
    }

    public async Task<Workshop> CreateAsync(string title, DateTime startTime, DateTime endTime, Guid venueId)
    {
        var workshop = new Workshop
        {
            Title = title,
            StartTime = startTime,
            EndTime = endTime,
            VenueId = venueId,
            RegisteredParticipants = 0
        };
        return await _repository.InsertAsync(workshop);
    }

    public async Task<Workshop> UpdateAsync(Guid id, string title, DateTime startTime, DateTime endTime, Guid venueId)
    {
        var workshop = await GetByIdNotNullAsync(id);

        if (workshop.RegisteredParticipants > 0)
            throw new InvalidOperationException($"Workshop with id {id} already has registered participants");

        workshop.Title = title;
        workshop.StartTime = startTime;
        workshop.EndTime = endTime;
        workshop.VenueId = venueId;
        return await _repository.UpdateAsync(workshop);
    }

    public async Task<Workshop> DeleteByIdAsync(Guid id)
    {
        var workshop = await GetByIdNotNullAsync(id);

        if (workshop.RegisteredParticipants > 0)
            throw new InvalidOperationException($"Workshop with id {id} already has registered participants");

        return await _repository.DeleteAsync(workshop);
    }

    public async Task<PaginatedResult<Workshop>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            include: x => x.Include(w => w.Enrollments).ThenInclude(e => e.User),
            pageNumber: pageNumber,
            pageSize: pageSize,
            asNoTracking: true);
    }

    public async Task IncrementParticipantsAsync(Guid id)
    {
        var workshop = await GetByIdNotNullAsync(id);
        workshop.RegisteredParticipants += 1;
        await _repository.UpdateAsync(workshop);
    }

    public async Task DecrementParticipantsAsync(Guid id)
    {
        var workshop = await GetByIdNotNullAsync(id);
        workshop.RegisteredParticipants -= 1;
        await _repository.UpdateAsync(workshop);
    }
}