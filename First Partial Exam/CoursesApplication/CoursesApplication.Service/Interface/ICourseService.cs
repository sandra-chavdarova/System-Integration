using CoursesApplication.Domain.Dto;
using CoursesApplication.Domain.Models;

namespace CoursesApplication.Service.Interface;

public interface ICourseService
{
    Task<List<Course>> GetAllAsync();
    Task<Course> GetByIdAsync(Guid id);
    Task<Course> InsertAsync(CourseDto dto);
    Task<Course> UpdateAsync(Guid id, CourseDto dto);
    Task<Course> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<Course>> GetAllPagedAsync(int pageNumber, int pageSize);

}