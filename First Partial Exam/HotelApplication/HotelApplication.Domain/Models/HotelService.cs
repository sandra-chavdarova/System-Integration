using HotelApplication.Domain.Common;
using HotelApplication.Domain.Enums;

namespace HotelApplication.Domain.Models;

public class HotelService : BaseEntity
{
    public ServiceType Type { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public int Duration { get; set; }
    
    public Guid? HotelId { get; set; }
    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; }

}