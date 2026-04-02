using CoursesApplication.Domain.Common;
using CoursesApplication.Domain.Enums;

namespace CoursesApplication.Domain.Models;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Ects { get; set; }
    public Category Category { get; set; }
    
    public Guid SemesterId { get; set; }
    public virtual Semester Semester { get; set; } = null;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}