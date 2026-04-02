using HotelApplication.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Repository;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<HotelService> HotelServices { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<HotelApplicationUser> Users { get; set; }
}