using System.Linq.Expressions;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

namespace TestExamIS.Tests.Utils;

public static class TestDatabaseHelper
{
    public static void SeedDatabase(ApplicationDbContext context, IPasswordHasher<WorkshopApplicationUser> passwordHasher)
    {
        var random = new Random(42);
        var roles = new[] { Role.Instructor, Role.Coordinator, Role.Facilitator };
        var firstNames = new[] { "Liam","Olivia","Noah","Emma","Oliver","Ava","Elijah","Sophia","James","Isabella" };
        var lastNames = new[] { "Smith","Johnson","Williams","Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez" };

        var venues = new List<Venue>
        {
            new() { Id = Guid.NewGuid(), Name = "Hall-A", Capacity = 30 },
            new() { Id = Guid.NewGuid(), Name = "Hall-B", Capacity = 50 },
            new() { Id = Guid.NewGuid(), Name = "Lab-1", Capacity = 25 },
            new() { Id = Guid.NewGuid(), Name = "Room-101", Capacity = 40 },
            new() { Id = Guid.NewGuid(), Name = "Studio-1", Capacity = 60 }
        };
        context.Venues.AddRange(venues);

        var users = new List<WorkshopApplicationUser>();
        for (int i = 0; i < 10; i++)
        {
            var fn = firstNames[i]; var ln = lastNames[i];
            var username = $"{fn.ToLower()}.{ln.ToLower()}{i}";
            var user = new WorkshopApplicationUser
            {
                Id = $"test-user-{i + 1}", UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = $"{username}@workshop.edu",
                NormalizedEmail = $"{username.ToUpper()}@WORKSHOP.EDU",
                EmailConfirmed = true, FirstName = fn, LastName = ln,
                Role = roles[i % roles.Length],
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");
            users.Add(user);
        }
        context.Users.AddRange(users);

        var now = DateTime.UtcNow;
        var workshops = new List<Workshop>();
        var titles = new[] { "Intro to C#","Advanced EF Core","Docker Basics","CI/CD Pipelines",
            "React Fundamentals","Cloud Architecture","Security Practices","API Design","Microservices","Testing" };
        for (int i = 0; i < 10; i++)
        {
            var start = now.Date.AddDays(random.Next(-30, 30)).AddHours(random.Next(8, 17));
            workshops.Add(new Workshop
            {
                Id = Guid.NewGuid(), Title = titles[i],
                StartTime = start, EndTime = start.AddHours(random.Next(1, 3)),
                VenueId = venues[i % venues.Count].Id, RegisteredParticipants = 0,
                CreatedAt = now.AddDays(-random.Next(1, 20)), CreatedById = users[0].Id,
                LastModifiedAt = now, LastModifiedById = users[0].Id
            });
        }
        context.Workshops.AddRange(workshops);

        var statuses = Enum.GetValues<EnrollmentStatus>();
        for (int i = 0; i < 15; i++)
        {
            var w = workshops[i % workshops.Count];
            var u = users[(i + 1) % users.Count];
            context.Enrollments.Add(new Enrollment
            {
                Id = Guid.NewGuid(), UserId = u.Id, VenueId = w.VenueId,
                WorkshopId = w.Id, Status = statuses[i % statuses.Length],
                Notes = i % 3 == 0 ? $"Test note {i}" : null
            });
        }

        for (int i = 0; i < 10; i++)
        {
            context.Assignments.Add(new Assignment
            {
                Id = Guid.NewGuid(), WorkshopId = workshops[i].Id, UserId = users[i].Id,
                CreatedAt = now.AddDays(-random.Next(1, 15)), CreatedById = users[0].Id,
                LastModifiedAt = now, LastModifiedById = users[0].Id
            });
        }
        context.SaveChanges();
    }

    public static async Task ResetDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<WorkshopApplicationUser>>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        SeedDatabase(context, hasher);
    }

    public static async Task<int> GetCount<T>(IServiceProvider sp) where T : class
    {
        using var scope = sp.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Set<T>().CountAsync();
    }

    public static async Task<T> GetFirst<T>(IServiceProvider sp) where T : class
    {
        using var scope = sp.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Set<T>().FirstAsync();
    }

    public static async Task<List<T>> GetAllWhere<T>(IServiceProvider sp, Expression<Func<T, bool>> pred) where T : class
    {
        using var scope = sp.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Set<T>().Where(pred).ToListAsync();
    }

    public static async Task<T?> GetFirstWhere<T>(IServiceProvider sp, Expression<Func<T, bool>> pred) where T : class
    {
        using var scope = sp.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Set<T>().Where(pred).FirstOrDefaultAsync();
    }

    public static async Task<T> CreateEntity<T>(IServiceProvider sp, T entity) where T : class
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Set<T>().AddAsync(entity);
        await db.SaveChangesAsync();
        return entity;
    }
}
