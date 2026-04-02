using HotelApplication.Domain.Common;

namespace HotelApplication.Domain.Models;

public class Reservation : BaseAuditableEntity<HotelApplicationUser>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ServiceDateTime { get; set; }
    
    public Guid? RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;
    
    public string? UserId { get; set; }
    public virtual HotelApplicationUser User { get; set; } = null!;
    
    public Guid? HotelServiceId { get; set; }
    public virtual HotelService HotelService { get; set; } = null!;
}