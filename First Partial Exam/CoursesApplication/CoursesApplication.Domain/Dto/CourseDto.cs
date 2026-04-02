using CoursesApplication.Domain.Enums;

namespace CoursesApplication.Domain.Dto;

public class CourseDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Ects { get; set; }
    public Category Category { get; set; }
    public Guid SemesterId { get; set; }
}