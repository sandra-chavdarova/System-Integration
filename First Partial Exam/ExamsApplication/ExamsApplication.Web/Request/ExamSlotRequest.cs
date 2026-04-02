namespace ExamsApplication.Web.Request;

public record ExamSlotRequest
(
    Guid ExamSlotId,
    DateTime StartTime,
    DateTime EndTime,
    Guid CourseId
);