namespace Web.Response;

public record ConsultationBasicResponse(
    Guid Id,
    DateTime Start,
    DateTime End,
    Guid RoomId
);