using ExamsApplication.Domain.Common;

namespace ExamsApplication.Domain.Models;

public class ExamAttempt : BaseEntity
{
    public int Grade { get; set; }
    
    public string UserId { get; set; }
    public virtual ExamsApplicationUser User { get; set; }
    
    public Guid ExamSlotId { get; set; }
    public virtual ExamSlot ExamSlot { get; set; }
    
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; }
}