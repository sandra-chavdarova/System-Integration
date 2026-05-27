using Repository;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web.DbSeeder;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<WorkshopApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();
        if (await context.Users.AnyAsync()) return;

        var rng = new Random(42);
        var roles = new[] { Role.Instructor, Role.Coordinator, Role.Facilitator };
        var firstNames = new[] { "Liam","Olivia","Noah","Emma","Oliver","Ava","Elijah","Sophia","James","Isabella" };
        var lastNames = new[] { "Smith","Johnson","Williams","Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez" };

        var users = new List<WorkshopApplicationUser>();
        for (int i = 0; i < 20; i++)
        {
            var fn = firstNames[i % firstNames.Length];
            var ln = lastNames[i % lastNames.Length];
            var username = $"{fn.ToLower()}.{ln.ToLower()}{i}";
            var user = new WorkshopApplicationUser
            {
                UserName = username, Email = $"{username}@workshop.edu", EmailConfirmed = true,
                FirstName = fn, LastName = ln, Role = roles[i % roles.Length]
            };
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded) users.Add(user);
        }
        await context.SaveChangesAsync();

        var venueNames = new[] { "Hall-A", "Hall-B", "Lab-1", "Lab-2", "Room-101", "Room-202", "Auditorium", "Studio-1" };
        var venues = venueNames.Select(name => new Venue { Id = Guid.NewGuid(), Name = name, Capacity = rng.Next(20, 100) }).ToList();
        await context.Venues.AddRangeAsync(venues);
        await context.SaveChangesAsync();

        var adminUser = users.First();
        var now = DateTime.UtcNow;
        var workshopTitles = new[] { "Intro to C#", "Advanced EF Core", "Docker Basics", "CI/CD Pipelines",
            "React Fundamentals", "Cloud Architecture", "Security Best Practices", "API Design",
            "Microservices", "Testing Strategies" };

        var workshops = new List<Workshop>();
        for (int i = 0; i < 10; i++)
        {
            var startOffset = rng.Next(-30, 30);
            var startHour = rng.Next(8, 17);
            var start = now.Date.AddDays(startOffset).AddHours(startHour);
            workshops.Add(new Workshop
            {
                Id = Guid.NewGuid(), Title = workshopTitles[i],
                StartTime = start, EndTime = start.AddHours(rng.Next(1, 3)),
                VenueId = venues[i % venues.Count].Id, RegisteredParticipants = 0,
                CreatedAt = now.AddDays(-rng.Next(1, 20)), CreatedById = adminUser.Id,
                LastModifiedAt = now, LastModifiedById = adminUser.Id
            });
        }
        await context.Workshops.AddRangeAsync(workshops);
        await context.SaveChangesAsync();

        var statuses = Enum.GetValues<EnrollmentStatus>();
        var enrollments = new List<Enrollment>();
        for (int i = 0; i < 15; i++)
        {
            var workshop = workshops[i % workshops.Count];
            var user = users[(i + 1) % users.Count];
            enrollments.Add(new Enrollment
            {
                Id = Guid.NewGuid(), UserId = user.Id, VenueId = workshop.VenueId,
                WorkshopId = workshop.Id, Status = statuses[i % statuses.Length],
                Notes = i % 3 == 0 ? $"Test note {i}" : null
            });
        }
        await context.Enrollments.AddRangeAsync(enrollments);

        var assignments = new List<Assignment>();
        for (int i = 0; i < 10; i++)
        {
            assignments.Add(new Assignment
            {
                Id = Guid.NewGuid(), WorkshopId = workshops[i % workshops.Count].Id,
                UserId = users[i % users.Count].Id,
                CreatedAt = now.AddDays(-rng.Next(1, 15)), CreatedById = adminUser.Id,
                LastModifiedAt = now, LastModifiedById = adminUser.Id
            });
        }
        await context.Assignments.AddRangeAsync(assignments);
        await context.SaveChangesAsync();
    }
}
