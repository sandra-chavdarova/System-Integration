using CoursesApplication.Domain.Models;
using CoursesApplication.Web.Response;

namespace CoursesApplication.Web.Extensions;

public static class EnrollmentMappingExtensions
{
    public static EnrollmentResponse ToResponse(this Enrollment e)
    {
        return new EnrollmentResponse(
            EnrollmentId: e.Id,
            UserId: e.UserId,
            UserName: string.Concat(str0: e.User.FirstName, str1: " ", str2: e.User.LastName),
            EnrollmentDate: e.EnrolledAt
        );
    }

    public static List<EnrollmentResponse> ToResponse(this List<Enrollment> enrollments)
    {
        return enrollments.Select(e => e.ToResponse()).ToList();
    }
}