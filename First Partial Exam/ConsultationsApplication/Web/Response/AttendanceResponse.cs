using Domain.Enums;

namespace Web.Response;

public record AttendanceResponse
(
    string UserName,
    Status Status
);