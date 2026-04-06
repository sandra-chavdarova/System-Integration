using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ConsultationMapper
{
    private readonly IConsultationService _service;

    public ConsultationMapper(IConsultationService service)
    {
        _service = service;
    }

    public async Task<List<ConsultationResponse>> GetAllAsync(string? roomName, DateOnly? date)
    {
        var result = await _service.GetAllAsync(roomName, date);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<ConsultationBasicResponse> InsertAsync(ConsultationRequest request)
    {
        var result = await _service.CreateAsync(startTime: request.StartTime, endTime: request.EndTime,
            roomId: request.RoomId);
        return result.ToBasicResponse();
    }

    public async Task<ConsultationBasicResponse> UpdateAsync(Guid id, ConsultationRequest request)
    {
        var result = await _service.UpdateAsync(id, request.StartTime, request.EndTime, request.RoomId);
        return result.ToBasicResponse();
    }

    public async Task<ConsultationBasicResponse> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);
        return result.ToBasicResponse();
    }

    public async Task<PaginatedResponse<ConsultationResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _service.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}