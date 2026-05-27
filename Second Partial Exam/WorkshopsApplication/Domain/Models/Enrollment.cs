using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Enrollment : BaseEntity
{
    public string? Notes { get; set; }
    public EnrollmentStatus Status { get; set; }

    public required string UserId { get; set; }
    public virtual WorkshopApplicationUser User { get; set; } = null!;

    public Guid VenueId { get; set; }
    public virtual Venue Venue { get; set; } = null!;

    public Guid WorkshopId { get; set; }
    public virtual Workshop Workshop { get; set; } = null!;

    public string? CancellationDocumentPath { get; set; }
}
