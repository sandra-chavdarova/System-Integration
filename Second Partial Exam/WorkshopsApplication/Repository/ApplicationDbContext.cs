using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<WorkshopApplicationUser>(options)
{
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<InboundEventEntry> InboundEventEntries { get; set; }
    public DbSet<ApiClient> ApiClients { get; set; }
    public DbSet<EtlSyncLog> EtlSyncLogs { get; set; }
}
