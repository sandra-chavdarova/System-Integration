namespace CoursesApplication.Web.Request;

public record EnrollmentRequest
(
    Guid EnrollmentId,
    string UserId,
    DateTime EnrollmentDate
);