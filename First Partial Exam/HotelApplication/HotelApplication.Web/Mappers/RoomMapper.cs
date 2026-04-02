using HotelApplication.Service.Interface;
using HotelApplication.Web.Extensions;
using HotelApplication.Web.Request;
using HotelApplication.Web.Response;

namespace HotelApplication.Web.Mappers;

public class RoomMapper
{
    public readonly IRoomService _service;

    public RoomMapper(IRoomService service)
    {
        _service = service;
    }

    public async Task<List<RoomResponse>> GetAllAsync(int status)
    {
        var result = await _service.GetAllAsync(status);
        return result.ToResponse();
    }

    public async Task<RoomResponse> GetByIdNotNullAsync(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<RoomResponse> InsertAsync(RoomRequest request)
    {
        var dto = request.CreateOrUpdateRoomRequest();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }

    public async Task<RoomResponse> UpdateAsync(Guid id, RoomRequest request)
    {
        var dto = request.CreateOrUpdateRoomRequest();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }

    public async Task<RoomResponse> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result?.ToResponse();
    }

    public async Task<PaginatedResponse<RoomResponse>> GetAllPagedAsync(PaginatedRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(r => r.ToResponse());
    }
}