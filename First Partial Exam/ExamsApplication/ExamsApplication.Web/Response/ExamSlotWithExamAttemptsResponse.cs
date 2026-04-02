using ExamsApplication.Web.Request;

namespace ExamsApplication.Web.Response;

public record ExamSlotWithExamAttemptsResponse(
    Guid ExamSlotId,
    DateTime StartTime,
    DateTime EndTime,
    Guid CourseId,
    string CourseName,
    List<ExamAttemptResponse> Responses
);