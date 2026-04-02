using HotelApplication.Domain.Models;
using HotelApplication.Web.Response;

namespace HotelApplication.Web.Extensions;

public static class ReservationMappingExtensions
{
    public static ReservationResponse ToResponse(this Reservation reservation)
    {
        return new ReservationResponse(
            StartDate: reservation.StartDate,
            EndDate: reservation.EndDate,
            ServiceDateTime: reservation.ServiceDateTime,
            UserName: string.Concat(str0: reservation.User.FirstName, str1: " ", str2: reservation.User.LastName),
            HotelServicePrice: reservation.HotelService.Price
        );
    }

    public static List<ReservationResponse> ToResponse(this List<Reservation> reservations)
    {
        return reservations.Select(r => r.ToResponse()).ToList();
    }
}