using Domain.Common;

namespace Domain.Models;

public class Workshop : BaseAuditableEntity<WorkshopApplicationUser>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Title { get; set; } = string.Empty;

    public Guid VenueId { get; set; }
    public virtual Venue Venue { get; set; } = null!;

    public int RegisteredParticipants { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
