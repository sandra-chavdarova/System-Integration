namespace Web.Request;

public record ConsultationRequest
(
    Guid ConsultationId,
    DateTime StartTime,
    DateTime EndTime,
    Guid RoomId
);