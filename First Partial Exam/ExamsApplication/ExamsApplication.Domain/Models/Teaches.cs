using ExamsApplication.Domain.Common;

namespace ExamsApplication.Domain.Models;

public class Teaches : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string UserId { get; set; }
    public virtual ExamsApplicationUser User { get; set; }
    
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; }
}