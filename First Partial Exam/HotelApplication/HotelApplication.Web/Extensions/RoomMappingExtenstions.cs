using HotelApplication.Domain.Dto;
using HotelApplication.Domain.Models;
using HotelApplication.Web.Request;
using HotelApplication.Web.Response;

namespace HotelApplication.Web.Extensions;

public static class RoomMappingExtenstions
{
    public static RoomResponse? ToResponse(this Room room)
    {
        return new RoomResponse(
            RoomNumber: room.RoomNumber,
            Capacity: room.Capacity,
            Status: room.Status.ToString(),
            PricePerNight: room.PricePerNight,
            HotelName: room.Hotel.Name
        );
    }

    public static RoomWithReservationsResponse? ToRoomWithReservationsResponse(this Room room)
    {
        return new RoomWithReservationsResponse(
            RoomNumber: room.RoomNumber,
            Capacity: room.Capacity,
            Status: room.Status.ToString(),
            PricePerNight: room.PricePerNight,
            HotelName: room.Hotel.Name,
            ReservationResponses: room.Reservations.ToList().ToResponse()
        );
    }

    public static List<RoomResponse?> ToResponse(this List<Room> rooms)
    {
        return rooms.Select(x => x.ToResponse()).ToList();
    }

    public static RoomDto CreateOrUpdateRoomRequest(this RoomRequest request)
    {
        return new RoomDto()
        {
            Capacity = request.Capacity,
            HotelId = request.HotelId,
            PricePerNight = request.PricePerNight,
            RoomNumber = request.RoomNumber,
            Status = request.Status
        };
    }

    public static PaginatedResponse<RoomWithReservationsResponse> ToPaginatedResponse(this PaginatedResult<Room> result)
    {
        return new PaginatedResponse<RoomWithReservationsResponse>()
        {
            Items = result.Items.Select(r => r.ToRoomWithReservationsResponse()).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}