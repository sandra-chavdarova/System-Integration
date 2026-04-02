using CoursesApplication.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoursesApplication.Repository;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CoursesApplicationUser> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Teaching> Teachings { get; set; }

}