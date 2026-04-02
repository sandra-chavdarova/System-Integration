using CoursesApplication.Web.Mapper;
using CoursesApplication.Web.Request;
using CoursesApplication.Web.Response;
using Microsoft.AspNetCore.Mvc;

namespace CoursesApplication.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly CourseMapper _mapper;

    public CourseController(CourseMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseResponse>> GetById([FromRoute] Guid id)
    {
        var result = await _mapper.GetById(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponse>>> GetAll()
    {
        var result = await _mapper.GetAll();
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseResponse>>> GetAllPages([FromQuery] PaginatedRequest request)
    {
        var result = await _mapper.GetAllPaged(request);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CourseResponse>> Create([FromBody] CourseRequest request)
    {
        var result = await _mapper.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseResponse>> Update([FromRoute] Guid id, [FromBody] CourseRequest request)
    {
        var result = await _mapper.Update(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CourseResponse>> Create([FromRoute] Guid id)
    {
        var result = await _mapper.Delete(id);
        return Ok(result);
    }
}