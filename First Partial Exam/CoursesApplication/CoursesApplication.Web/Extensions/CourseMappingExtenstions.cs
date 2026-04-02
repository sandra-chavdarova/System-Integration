using CoursesApplication.Domain.Dto;
using CoursesApplication.Domain.Models;
using CoursesApplication.Web.Request;
using CoursesApplication.Web.Response;

namespace CoursesApplication.Web.Extensions;

public static class CourseMappingExtensions
{
    public static CourseResponse ToResponse(this Course c)
    {
        return new CourseResponse(
            CourseId: c.Id,
            Name: c.Title,
            Description: c.Description,
            Ects: c.Ects,
            Category: c.Category,
            SemesterId: c.SemesterId,
            SemesterName: c.Semester.Name
        );
    }

    public static CourseWithEnrollmentsResponse CourseWithEnrollmentsToResponse(this Course c)
    {
        return new CourseWithEnrollmentsResponse(
            CourseId: c.Id,
            Name: c.Title,
            Description: c.Description,
            Ects: c.Ects,
            Category: c.Category,
            SemesterId: c.SemesterId,
            SemesterName: c.Semester.Name,
            EnrollmentResponses: c.Enrollments.ToList().ToResponse()
        );
    }

    public static List<CourseResponse> ToResponse(this List<Course> courses)
    {
        return courses.Select(c => c.ToResponse()).ToList();
    }

    public static CourseDto CreateOrUpdateCourseRequest(this CourseRequest request)
    {
        return new CourseDto()
        {
            Category = request.Category,
            Description = request.Description,
            Ects = request.Ects,
            SemesterId = request.SemesterId,
            Title = request.Name
        };
    }

    public static PaginatedResponse<CourseWithEnrollmentsResponse> ToPaginatedResponse(
        this PaginatedResult<Course> result)
    {
        return new PaginatedResponse<CourseWithEnrollmentsResponse>()
        {
            Items = result.Items.Select(r => r.CourseWithEnrollmentsToResponse()).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };
    }
}