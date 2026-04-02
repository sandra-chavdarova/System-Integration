using HotelApplication.Domain.Dto;
using HotelApplication.Domain.Models;
using HotelApplication.Repository.Interface;
using HotelApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Service.Implementation;

public class RoomService : IRoomService
{
    public readonly IRepository<Room> _repository;

    public RoomService(IRepository<Room> repository)
    {
        _repository = repository;
    }

    public async Task<List<Room>> GetAllAsync(int status)
    {
        if (status == null)
        {
            throw new Exception("Status is null");
        }

        var result = await _repository.GetAllAsync(selector: x => x, predicate: x => x.Status.Equals(status));
        return result.ToList();
    }

    public async Task<Room> GetByIdNotNullAsync(Guid id)
    {
        var result = await _repository.Get(selector: x => x, predicate: x => x.Id == id);
        if (result == null)
        {
            throw new Exception("Room doesn't exist");
        }

        return result;
    }

    public async Task<Room> InsertAsync(RoomDto dto)
    {
        var room = new Room()
        {
            Status = dto.Status,
            Capacity = dto.Capacity,
            RoomNumber = dto.RoomNumber,
            PricePerNight = dto.PricePerNight,
            HotelId = dto.HotelId
        };
        return await _repository.InsertAsync(room);
    }

    public async Task<Room> UpdateAsync(Guid id, RoomDto dto)
    {
        var room = await GetByIdNotNullAsync(id);
        room.Status = dto.Status;
        room.Capacity = dto.Capacity;
        room.RoomNumber = dto.RoomNumber;
        room.PricePerNight = dto.PricePerNight;
        room.HotelId = dto.HotelId;
        return await _repository.UpdateAsync(room);
    }

    public async Task<Room> DeleteAsync(Guid id)
    {
        var room = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(room);
    }

    public async Task<PaginatedResult<Room>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(y => y.Reservations),
            asNoTracking: true);
    }
}