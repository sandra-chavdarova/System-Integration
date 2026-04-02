using ExamsApplication.Domain.Enum;
using ExamsApplication.Domain.Models;

namespace ExamsApplication.Domain.Dto;

public class ExamSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SessionType SessionType { get; set; }
    public Guid CourseId { get; set; }
}