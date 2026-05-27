namespace Web.Request;

public record WorkshopRequest(Guid VenueId, string Title, DateTime StartTime, DateTime EndTime);
