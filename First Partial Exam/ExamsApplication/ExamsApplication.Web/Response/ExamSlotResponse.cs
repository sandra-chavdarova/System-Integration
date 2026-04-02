namespace ExamsApplication.Web.Response;

public record ExamSlotResponse
(
    Guid ExamSlotId,
    DateTime StartTime,
    DateTime EndTime,
    Guid CourseId,
    string CourseName
);