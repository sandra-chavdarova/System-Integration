using CoursesApplication.Domain.Dto;
using CoursesApplication.Domain.Models;
using CoursesApplication.Repository.Interface;
using CoursesApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace CoursesApplication.Service.Implementation;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _repository;

    public CourseService(IRepository<Course> repository)
    {
        _repository = repository;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(selector: x => x);
        return result.ToList();
    }

    public async Task<Course> GetByIdAsync(Guid id)
    {
        var result = await _repository.Get(selector: x => x, predicate: x => x.Id.Equals(id));
        if (result == null)
        {
            throw new Exception("Course doesn't exist");
        }

        return result;
    }

    public async Task<Course> InsertAsync(CourseDto dto)
    {
        var course = new Course()
        {
            Category = dto.Category,
            Description = dto.Description,
            Title = dto.Title,
            Ects = dto.Ects,
            SemesterId = dto.SemesterId
        };
        return await _repository.InsertAsync(course);
    }

    public async Task<Course> UpdateAsync(Guid id, CourseDto dto)
    {
        var course = await GetByIdAsync(id);
        course.Category = dto.Category;
        course.Description = dto.Description;
        course.Title = dto.Title;
        course.Ects = dto.Ects;
        course.SemesterId = dto.SemesterId;
        return await _repository.UpdateAsync(course);
    }

    public async Task<Course> DeleteAsync(Guid id)
    {
        var course = await GetByIdAsync(id);
        return await _repository.DeleteAsync(course);
    }

    public async Task<PaginatedResult<Course>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x => x.Include(y => y.Enrollments),
            asNoTracking: true
        );
    }
}