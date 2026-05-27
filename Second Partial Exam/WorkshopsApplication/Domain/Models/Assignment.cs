using Domain.Common;

namespace Domain.Models;

public class Assignment : BaseAuditableEntity<WorkshopApplicationUser>
{
    public Guid WorkshopId { get; set; }
    public virtual Workshop Workshop { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public virtual WorkshopApplicationUser User { get; set; } = null!;
}
