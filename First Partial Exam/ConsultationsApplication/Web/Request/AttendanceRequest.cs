using Domain.Enums;

namespace Web.Request;

public record AttendanceRequest
(
    Guid AttendanceId,
    string UserId
);