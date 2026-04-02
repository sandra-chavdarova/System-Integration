using CoursesApplication.Domain.Enums;

namespace CoursesApplication.Web.Response;

public record CourseResponse
(
    Guid CourseId,
    string Name,
    string Description,
    int Ects,
    Category Category,
    Guid SemesterId,
    string SemesterName
);