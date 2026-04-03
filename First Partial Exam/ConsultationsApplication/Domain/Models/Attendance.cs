using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Attendance : BaseEntity
{
    public string Comment { get; set; }
    public Status Status { get; set; }
    
    public string UserId { get; set; }
    public virtual ConsultationsApplicationUser User { get; set; }
    
    public Guid RoomId { get; set; }
    public virtual Room Room { get; set; }
    
    public Guid ConsultationId { get; set; }
    public virtual Consultation Consultation { get; set; }
}