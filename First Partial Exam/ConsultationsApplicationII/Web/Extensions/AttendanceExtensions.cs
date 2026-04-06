using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class AttendanceExtensions
{
    public static AttendanceBasicResponse ToBasicResponse(this Attendance a)
    {
        return new AttendanceBasicResponse(
            id: a.Id,
            FirstName: a.User?.FirstName,
            LastName: a.User?.LastName
        );
    }

    public static AttendanceResponse ToResponse(this Attendance attendance)
    {
        return new AttendanceResponse(
            attendance.UserId,
            attendance.User?.FirstName,
            attendance.User?.LastName,
            attendance.Status.ToString(),
            attendance.Comment
        );
    }
}