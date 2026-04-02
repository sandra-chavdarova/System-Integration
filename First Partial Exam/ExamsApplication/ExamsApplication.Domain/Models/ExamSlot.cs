using ExamsApplication.Domain.Common;
using ExamsApplication.Domain.Enum;

namespace ExamsApplication.Domain.Models;

public class ExamSlot : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SessionType SessionType { get; set; }

    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; }

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; }
}