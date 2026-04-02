using Microsoft.AspNetCore.Identity;

namespace CoursesApplication.Domain.Models;

public class CoursesApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
}