using HotelApplication.Domain.Enums;

namespace HotelApplication.Web.Response;

public record RoomWithReservationsResponse
(
    int RoomNumber,
    int Capacity,
    string Status,
    double PricePerNight,
    string HotelName,
    List<ReservationResponse> ReservationResponses
);