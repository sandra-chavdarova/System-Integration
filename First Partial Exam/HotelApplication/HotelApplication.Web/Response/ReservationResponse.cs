namespace HotelApplication.Web.Response;

public record ReservationResponse
(
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ServiceDateTime,
    string UserName,
    double HotelServicePrice
);