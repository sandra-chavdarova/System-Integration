using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class WorkshopMapper
{
    private readonly IWorkshopService _workshopService;
    public WorkshopMapper(IWorkshopService workshopService) { _workshopService = workshopService; }

    public async Task<List<WorkshopResponse>> GetAllAsync(string? venueName, DateOnly? date)
    {
        var result = await _workshopService.GetAllAsync(venueName, date);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task<WorkshopBasicResponse> InsertAsync(WorkshopRequest request)
    {
        var result = await _workshopService.CreateAsync(request.Title, request.StartTime, request.EndTime, request.VenueId);
        return result.ToBasicResponse();
    }

    public async Task<WorkshopBasicResponse> UpdateAsync(Guid id, WorkshopRequest request)
    {
        var result = await _workshopService.UpdateAsync(id, request.Title, request.StartTime, request.EndTime, request.VenueId);
        return result.ToBasicResponse();
    }

    public async Task<WorkshopBasicResponse> DeleteAsync(Guid id)
    {
        var result = await _workshopService.DeleteByIdAsync(id);
        return result.ToBasicResponse();
    }

    public async Task<PaginatedResponse<WorkshopResponse>> GetAllPaginatedAsync(PaginatedRequest request)
    {
        var result = await _workshopService.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(x => x.ToResponse());
    }
}
