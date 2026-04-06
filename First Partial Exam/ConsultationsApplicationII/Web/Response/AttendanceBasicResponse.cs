namespace Web.Response;

public record AttendanceBasicResponse(
    Guid id,
    string FirstName,
    string LastName
);