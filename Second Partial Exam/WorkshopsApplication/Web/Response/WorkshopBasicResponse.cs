namespace Web.Response;

public record WorkshopBasicResponse(Guid Id, Guid VenueId, string Title, DateTime Start, DateTime End);
