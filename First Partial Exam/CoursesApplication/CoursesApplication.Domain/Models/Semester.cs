using CoursesApplication.Domain.Common;

namespace CoursesApplication.Domain.Models;

public class Semester : BaseAuditableEntity<CoursesApplicationUser>
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public virtual ICollection<Teaching> Teachers { get; set; } = new List<Teaching>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}