using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConsultationController : ControllerBase
{
    private readonly ConsultationMapper _mapper;

    public ConsultationController(ConsultationMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ConsultationResponse>> RegisterAsync([FromBody] ConsultationRequest request)
    {
        var result = await _mapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ConsultationResponse>>> GetAllAsync([FromQuery] string? roomNumber,
        [FromQuery] DateOnly? date)
    {
        var result = await _mapper.GetAllAsync(roomNumber, date);
        return Ok(result);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<ConsultationBasicResponse>> UpdateAsync([FromRoute] Guid id,
        [FromBody] ConsultationRequest request)
    {
        var result = await _mapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ConsultationBasicResponse>> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(id);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PaginatedResponse<ConsultationResponse>>> GetAllPaged(
        [FromQuery] PaginatedRequest request)
    {
        var result = await _mapper.GetAllPaginatedAsync(request);
        return Ok(result);
    }
}