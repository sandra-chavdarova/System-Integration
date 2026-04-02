using CoursesApplication.Service.Interface;
using CoursesApplication.Web.Extensions;
using CoursesApplication.Web.Request;
using CoursesApplication.Web.Response;

namespace CoursesApplication.Web.Mapper;

public class CourseMapper
{
    private readonly ICourseService _service;

    public CourseMapper(ICourseService service)
    {
        _service = service;
    }

    public async Task<List<CourseResponse>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<CourseResponse> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.ToResponse();
    }

    public async Task<CourseResponse> Create(CourseRequest request)
    {
        var dto = request.CreateOrUpdateCourseRequest();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }

    public async Task<CourseResponse> Update(Guid id, CourseRequest request)
    {
        var dto = request.CreateOrUpdateCourseRequest();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }

    public async Task<CourseResponse> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<CourseResponse>> GetAllPaged(PaginatedRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(p => p.ToResponse());
    }
}