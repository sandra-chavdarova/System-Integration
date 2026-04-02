using HotelApplication.Domain.Common;
using HotelApplication.Domain.Enums;

namespace HotelApplication.Domain.Models;

public class Room : BaseEntity
{
    public Status Status { get; set; }
    public int Capacity { get; set; }
    public int RoomNumber { get; set; }
    public double PricePerNight { get; set; }

    public Guid? HotelId { get; set; }
    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; }
}