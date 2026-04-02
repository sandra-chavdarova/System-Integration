using ExamsApplication.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace ExamsApplication.Domain.Models;

public class ExamsApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Role Role { get; set; }
    
    public virtual ICollection<Teaches> Teaches { get; set; }
    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; }
}