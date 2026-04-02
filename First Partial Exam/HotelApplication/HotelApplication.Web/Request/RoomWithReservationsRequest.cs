using HotelApplication.Domain.Enums;

namespace HotelApplication.Web.Request;

public record RoomWithReservationsRequest(
    Guid RoomId,
    int RoomNumber,
    int Capacity,
    Status Status,
    double PricePerNight,
    Guid HotelId,
    string HotelName,
    List<ReservationResponse> ReservationResponses
);