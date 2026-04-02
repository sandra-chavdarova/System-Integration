namespace HotelApplication.Web.Request;

public record ReservationResponse(
    Guid ReservationId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ServiceDateTime,
    string UserId,
    string UserName,
    Guid HotelServiceId,
    double HotelServicePrice
);