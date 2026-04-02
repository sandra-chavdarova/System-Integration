using ExamsApplication.Domain.Common;

namespace ExamsApplication.Domain.Models;

public class Course : BaseEntity
{
    public string Title { get; set; }
    public decimal Credits { get; set; }
    
    public virtual ICollection<Teaches> Teaches { get; set; }
    public virtual ICollection<ExamSlot> ExamSlots { get; set; }
    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; }
}