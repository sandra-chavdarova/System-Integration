using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class EnrollmentExtensions
{
    public static EnrollmentResponse ToResponse(this Enrollment e) =>
        new(e.UserId, e.User.FirstName, e.User.LastName, e.Status.ToString(), e.Notes);

    public static EnrollmentBasicResponse ToBasicResponse(this Enrollment e) =>
        new(e.Id, e.User.FirstName, e.User.LastName);
}
