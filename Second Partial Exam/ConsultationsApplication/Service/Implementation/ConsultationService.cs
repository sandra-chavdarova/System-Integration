using Domain.Configuration;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ConsultationService : IConsultationService
{
    private readonly IRepository<Consultation> _consultationRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ConsultationService> _logger;
    private readonly CacheSettings _cacheSettings;


    public ConsultationService(IRepository<Consultation> consultationRepository, IMemoryCache memoryCache,
        ILogger<ConsultationService> logger, IOptions<CacheSettings> cacheSettings)
    {
        _consultationRepository = consultationRepository;
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<Consultation> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Consultation with id {id} not found");
        }

        return result;
    }

    public async Task<Consultation?> GetByIdAsync(Guid id)
    {
        return await _consultationRepository.GetAsync(
            selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<List<Consultation>> GetAllAsync(string? roomName, DateOnly? date)
    {
        var cacheKey = $"consultations:{roomName}:{date:yyyy-MM-dd}";

        // Check cache
        if (_memoryCache.TryGetValue(cacheKey, out List<Consultation>? cached))
        {
            _logger.LogDebug(
                $"Cache hit for room {roomName}", roomName);
            return cached;
        }

        // If present, return
        if (cached != null)
        {
            return cached;
        }

        // If not present, fetch
        var apiData = await _consultationRepository.GetAllAsync(
            selector: x => x,
            predicate: x => (roomName == null || x.Room.Name.Contains(roomName)) &&
                            (date == null || DateOnly.FromDateTime(x.StartTime) == date),
            include: x => x.Include(c => c.Attendances).ThenInclude(a => a.User));

        // Put in the cache
        _memoryCache.Set(cacheKey, apiData, TimeSpan.FromMinutes(_cacheSettings.ListCacheDurationMinutes));
        return apiData;
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

        return await _consultationRepository.InsertAsync(consultation);
    }

    public async Task<Consultation> UpdateAsync(Guid id, DateTime startTime, DateTime endTime, Guid roomId)
    {
        var consultation = await GetByIdNotNullAsync(id);

        if (consultation.RegisteredStudents > 0)
        {
            throw new InvalidOperationException($"Consultation with id {id} already has registered students");
        }

        consultation.StartTime = startTime;
        consultation.EndTime = endTime;
        consultation.RoomId = roomId;
        return await _consultationRepository.UpdateAsync(consultation);
    }

    public async Task<Consultation> DeleteByIdAsync(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);

        if (consultation.RegisteredStudents > 0)
        {
            throw new InvalidOperationException($"Consultation with id {id} already has registered students");
        }

        return await _consultationRepository.DeleteAsync(consultation);
    }

    public async Task<PaginatedResult<Consultation>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _consultationRepository.GetAllPagedAsync(
            selector: x => x,
            include: x => x.Include(c => c.Attendances).ThenInclude(a => a.User),
            pageNumber: pageNumber,
            pageSize: pageSize,
            asNoTracking: true);
    }

    public async Task IncrementNumberOfStudentsAsync(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        consultation.RegisteredStudents += 1;
        await _consultationRepository.UpdateAsync(consultation);
    }

    public async Task DecrementNumberOfStudentsAsync(Guid id)
    {
        var consultation = await GetByIdNotNullAsync(id);
        consultation.RegisteredStudents -= 1;
        await _consultationRepository.UpdateAsync(consultation);
    }
}