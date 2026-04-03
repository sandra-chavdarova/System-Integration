using Domain.Enums;
using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class AttendanceMappingExtensions
{
    public static AttendanceResponse ToResponse(this Attendance a)
    {
        return new AttendanceResponse(
            UserName: string.Concat(str0: a.User.FirstName, str1: " ", str2: a.User.LastName),
            Status: a.Status.ToString()
        );
    }

    public static List<AttendanceResponse> ToResponse(this List<Attendance> attendances)
    {
        return attendances.Select(a => a.ToResponse()).ToList();
    }
}
