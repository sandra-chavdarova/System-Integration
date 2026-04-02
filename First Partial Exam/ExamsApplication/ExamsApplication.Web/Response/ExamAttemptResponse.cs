namespace ExamsApplication.Web.Request;

public record ExamAttemptResponse
(
    Guid ExamAttemptId,
    string UserId,
    string UserName,
    int Grade
);