namespace Web.Response;

public record ConsultationWithAttendancesResponse
(
    DateTime StartTime,
    DateTime EndTime,
    string RoomName,
    List<AttendanceResponse> AttendanceResponses
);