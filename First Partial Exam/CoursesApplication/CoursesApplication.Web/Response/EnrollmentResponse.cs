namespace CoursesApplication.Web.Response;

public record EnrollmentResponse(
    Guid EnrollmentId,
    string UserId,
    string UserName,
    DateTime EnrollmentDate
);