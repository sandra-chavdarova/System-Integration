using Microsoft.AspNetCore.Identity;

namespace HotelApplication.Domain.Models;

public class HotelApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; }
}