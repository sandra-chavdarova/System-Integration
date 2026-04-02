using HotelApplication.Domain.Dto;
using HotelApplication.Domain.Models;

namespace HotelApplication.Service.Interface;

public interface IRoomService
{
    Task<List<Room>> GetAllAsync(int status);
    Task<Room> GetByIdNotNullAsync(Guid id);
    Task<Room> InsertAsync(RoomDto dto);
    Task<Room> UpdateAsync(Guid id, RoomDto dto);
    Task<Room> DeleteAsync(Guid id);

    public Task<PaginatedResult<Room>> GetAllPagedAsync(int pageNumber, int pageSize);
}