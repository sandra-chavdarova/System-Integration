namespace Web.Response;

public record ConsultationResponse
(
    DateTime StartTime,
    DateTime EndTime,
    string RoomName
);