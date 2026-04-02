using HotelApplication.Domain.Enums;

namespace HotelApplication.Web.Response;

public record RoomResponse
(
    int RoomNumber,
    int Capacity,
    string Status,
    double PricePerNight,
    string HotelName
);