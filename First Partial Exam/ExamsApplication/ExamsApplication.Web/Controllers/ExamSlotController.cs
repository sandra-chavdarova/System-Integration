using ExamsApplication.Web.Mapper;
using ExamsApplication.Web.Request;
using ExamsApplication.Web.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExamsApplication.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamSlotController : ControllerBase
{
    private readonly ExamSlotMapper _mapper;

    public ExamSlotController(ExamSlotMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mapper.GetByIdNotNull(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ExamSlotResponse>>> GetAll([FromQuery] string? sessionType)
    {
        var result = await _mapper.GetAll(sessionType);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ExamSlotResponse>>> GetPaged([FromQuery] PaginatedRequest request)
    {
        var result = await _mapper.GetAllPaged(request);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ExamSlotResponse>> Create([FromBody] ExamSlotRequest request)
    {
        var result = await _mapper.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExamSlotResponse>> Update([FromRoute] Guid id, [FromBody] ExamSlotRequest request)
    {
        var result = await _mapper.Update(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ExamSlotResponse>> Update([FromRoute] Guid id)
    {
        var result = await _mapper.Delete(id);
        return Ok(result);
    }
}