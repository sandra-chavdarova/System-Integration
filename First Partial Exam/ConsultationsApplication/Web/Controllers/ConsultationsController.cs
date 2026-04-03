using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsultationsController : ControllerBase
{
    private readonly ConsultationMapper _mapper;

    public ConsultationsController(ConsultationMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mapper.GetById(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? dateAfter)
    {
        var result = await _mapper.GetAll(dateAfter);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult> GetPaged([FromQuery] PaginatedRequest request)
    {
        var result = await _mapper.GetPaged(request);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromBody] ConsultationRequest request)
    {
        var result = await _mapper.Create(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] ConsultationRequest request)
    {
        var result = await _mapper.Update(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mapper.Delete(id);
        return Ok(result);
    }
}