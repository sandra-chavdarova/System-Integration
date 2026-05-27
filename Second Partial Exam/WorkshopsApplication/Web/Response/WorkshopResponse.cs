namespace Web.Response;

public record WorkshopResponse(Guid Id, DateOnly Date, string Title, Guid VenueId, int RegisteredParticipants, List<EnrollmentBasicResponse> Enrollments);
