namespace Web.Response;

public record EnrollmentResponse(string UserId, string FirstName, string LastName, string Status, string? Notes);
