using ExamsApplication.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamsApplication.Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ExamsApplicationUser>(options)
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<ExamSlot> ExamSlots { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }
    public DbSet<Teaches> Teaches { get; set; }
}