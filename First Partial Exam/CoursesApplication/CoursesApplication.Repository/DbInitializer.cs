using Microsoft.AspNetCore.Identity;
using CoursesApplication.Domain.Models;
using CoursesApplication.Domain.Enums;
using CoursesApplication.Repository;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<CoursesApplicationUser> userManager)
    {
        if (context.Courses.Any()) return;
        
        var adminEmail = "admin@university.edu";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        
        if (admin == null)
        {
            admin = new CoursesApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                DateOfBirth = new DateOnly(1990, 1, 1),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Password123!");
        }
        
        // --- PROFESSORS ---
        var prof1 = await CreateUserAsync(userManager, "alice.johnson@university.edu", "Alice", "Johnson", new DateOnly(1975, 4, 12));
        var prof2 = await CreateUserAsync(userManager, "robert.smith@university.edu", "Robert", "Smith", new DateOnly(1968, 9, 30));

        // --- ASSISTANTS ---
        var asst1 = await CreateUserAsync(userManager, "maria.kovac@university.edu", "Maria", "Kovac", new DateOnly(1990, 6, 15));
        var asst2 = await CreateUserAsync(userManager, "david.lee@university.edu", "David", "Lee", new DateOnly(1992, 11, 3));
        
        // --- DEMONSTRATORS ---
        var demo1 = await CreateUserAsync(userManager, "stefan.trajkovski@uni.edu", "Stefan", "Trajkovski", new DateOnly(2002, 5, 10));
        var demo2 = await CreateUserAsync(userManager, "ana.petrovska@uni.edu", "Ana", "Petrovska", new DateOnly(2003, 1, 20));

        // --- TUTORS ---
        var tutor1 = await CreateUserAsync(userManager, "igor.stojanovski@uni.edu", "Igor", "Stojanovski", new DateOnly(2004, 8, 15));
        var tutor2 = await CreateUserAsync(userManager, "elena.miteva@uni.edu", "Elena", "Miteva", new DateOnly(2003, 3, 5));

        // --- STUDENTS ---
        var stud1 = await CreateUserAsync(userManager, "emily.brown@student.edu", "Emily", "Brown", new DateOnly(2004, 2, 20));
        var stud2 = await CreateUserAsync(userManager, "james.wilson@student.edu", "James", "Wilson", new DateOnly(2005, 7, 8));
        var stud3 = await CreateUserAsync(userManager, "john.doe@student.edu", "John", "Doe", new DateOnly(2003, 5, 5));
        var stud4 = await CreateUserAsync(userManager, "jane.doe@student.edu", "Jane", "Doe", new DateOnly(2003, 5, 5));


        
        var summerSemester = new Semester
        {
            Id = Guid.NewGuid(),
            Name = "Летен Семестар 2026/27",
            StartDate = new DateTime(2026, 2, 15),
            EndDate = new DateTime(2026, 6, 15),
            CreatedById = admin.Id,
            LastModifiedById = admin.Id,
            DateCreated = DateTime.UtcNow
        };
        
        var winterSemester = new Semester
        {
            Id = Guid.NewGuid(),
            Name = "Зимски Семестар 2025/26",
            StartDate = new DateTime(2025, 10, 1),
            EndDate = new DateTime(2026, 1, 31),
            CreatedById = admin.Id,
            LastModifiedById = admin.Id,
            DateCreated = DateTime.UtcNow
        };
        
        var winterSemesterNext = new Semester
        {
            Id = Guid.NewGuid(),
            Name = "Зимски Семестар 2026/27",
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2027, 1, 31),
            CreatedById = admin.Id,
            LastModifiedById = admin.Id,
            DateCreated = DateTime.UtcNow
        };
        
        context.Semesters.AddRange(summerSemester, winterSemester, winterSemesterNext);
        
        var course1 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Објектно Ориентирано Програмирање",
            Description = "Основи на C++ и класи.",
            Ects = 6,
            Category = Category.Programming,
            SemesterId = winterSemester.Id
        };

        var course2 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Дискретна Математика",
            Description = "Логика и графови.",
            Ects = 6,
            Category = Category.Mathematics,
            SemesterId = winterSemester.Id
        };
        
        var course3 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Интегрирани системи",
            Description = "C# и .NET Core",
            Ects = 6,
            Category = Category.Programming,
            SemesterId = summerSemester.Id
        };
        
        var course4 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Е-Влада",
            Description = "Основи на Е-Влада",
            Ects = 6,
            Category = Category.Business,
            SemesterId = winterSemesterNext.Id
        };
        
        var course5 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Калкулус",
            Description = "Математика",
            Ects = 6,
            Category = Category.Mathematics,
            SemesterId = winterSemesterNext.Id
        };
        
        var course6 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Основи на веб дизајн",
            Description = "HTML/CSS",
            Ects = 6,
            Category = Category.Programming,
            SemesterId = summerSemester.Id
        };
        
        context.Courses.AddRange(course1, course2,course3, course4, course5, course6);
        
        
        var teachingAssignments = new List<Teaching>
        {
            // Објектно Ориентирано Програмирање
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof1.Id, CourseId = course1.Id, 
                SemesterId = winterSemester.Id, Role = Role.Professor 
            },
            new Teaching { 
                Id = Guid.NewGuid(), UserId = asst1.Id, CourseId = course1.Id, 
                SemesterId = winterSemester.Id, Role = Role.Assistant 
            },
            new Teaching { 
                Id = Guid.NewGuid(), UserId = demo1.Id, CourseId = course1.Id, 
                SemesterId = winterSemester.Id, Role = Role.Demonstrator 
            },

            // Дискретна Математика
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof2.Id, CourseId = course2.Id, 
                SemesterId = winterSemester.Id, Role = Role.Professor 
            },
            new Teaching { 
                Id = Guid.NewGuid(), UserId = asst2.Id, CourseId = course2.Id, 
                SemesterId = winterSemester.Id, Role = Role.Assistant 
            },
            
            // Интегрирани системи
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof1.Id, CourseId = course3.Id, 
                SemesterId = summerSemester.Id, Role = Role.Professor 
            },
            new Teaching { 
                Id = Guid.NewGuid(), UserId = tutor1.Id, CourseId = course3.Id, 
                SemesterId = summerSemester.Id, Role = Role.Tutor 
            },

            // Основи на веб дизајн
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof1.Id, CourseId = course6.Id, 
                SemesterId = summerSemester.Id, Role = Role.Professor 
            },
            
            // Е-Влада
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof2.Id, CourseId = course4.Id, 
                SemesterId = winterSemesterNext.Id, Role = Role.Professor 
            },
            // Калкулус
            new Teaching { 
                Id = Guid.NewGuid(), UserId = prof2.Id, CourseId = course5.Id, 
                SemesterId = winterSemesterNext.Id, Role = Role.Professor 
            },
            new Teaching { 
                Id = Guid.NewGuid(), UserId = tutor2.Id, CourseId = course5.Id, 
                SemesterId = winterSemesterNext.Id, Role = Role.Tutor 
            }
        };

        context.Teachings.AddRange(teachingAssignments);
        
        var enrollments = new List<Enrollment>
        {
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud1.Id, CourseId = course1.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud1.Id, CourseId = course2.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud2.Id, CourseId = course6.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud2.Id, CourseId = course3.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud3.Id, CourseId = course4.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud3.Id, CourseId = course6.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            
            new Enrollment { 
                Id = Guid.NewGuid(), UserId = stud4.Id, CourseId = course5.Id, 
                EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            },
            new Enrollment { 
            Id = Guid.NewGuid(), UserId = stud4.Id, CourseId = course1.Id, 
            EnrolledAt = DateTime.UtcNow, CreatedById = admin.Id, LastModifiedById = admin.Id, DateCreated = DateTime.UtcNow 
            }
        };
        

        context.Enrollments.AddRange(enrollments);
        
        await context.SaveChangesAsync();
    }
    
    static async Task<CoursesApplicationUser> CreateUserAsync(UserManager<CoursesApplicationUser> mgr, string email, string first, string last, DateOnly dob)
    {
        var user = await mgr.FindByEmailAsync(email);
        if (user == null)
        {
            user = new CoursesApplicationUser
            {
                UserName = email.Split('@')[0], 
                Email = email,
                FirstName = first,
                LastName = last,
                DateOfBirth = dob,
                EmailConfirmed = true
            };
            var result = await mgr.CreateAsync(user, "Test123!");
            
        }
        return user;
    }
}