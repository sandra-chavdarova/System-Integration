using CoursesApplication.Domain.Enums;

namespace CoursesApplication.Web.Request;

public record CourseRequest
(
    Guid CourseId,
    string Name,
    string Description,
    int Ects,
    Category Category,
    Guid SemesterId
);