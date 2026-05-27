namespace Web.Request;

public record EnrollmentRequest(Guid WorkshopId, string UserId, Guid VenueId, string? Notes);
