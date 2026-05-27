using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class WorkshopApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Role Role { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
