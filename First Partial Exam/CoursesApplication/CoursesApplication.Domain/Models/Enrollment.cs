using CoursesApplication.Domain.Common;

namespace CoursesApplication.Domain.Models;

public class Enrollment : BaseAuditableEntity<CoursesApplicationUser>
{
    public DateTime EnrolledAt { get; set; } 
    
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null;
    
    public string UserId { get; set; }
    public virtual CoursesApplicationUser User { get; set; } = null;
}