using HotelApplication.Domain.Common;

namespace HotelApplication.Domain.Models;

public class Hotel : BaseEntity
{
    public string Name { get; set; }
    public string Address { get; set; }

    public virtual ICollection<HotelService> HotelServices { get; set; }
    public virtual ICollection<Room> Rooms { get; set; }
}