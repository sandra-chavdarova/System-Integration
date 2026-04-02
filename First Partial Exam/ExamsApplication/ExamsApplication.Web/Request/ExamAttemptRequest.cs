namespace ExamsApplication.Web.Request;

public record ExamAttemptRequest
(
    Guid ExamAttemptId,
    string UserId,
    int Grade
);