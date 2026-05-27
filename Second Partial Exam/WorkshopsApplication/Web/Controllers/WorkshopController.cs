using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;
using Web.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkshopController : ControllerBase
{
    private readonly WorkshopMapper _workshopMapper;

    public WorkshopController(WorkshopMapper workshopMapper) { _workshopMapper = workshopMapper; }

    [HttpGet("")]
    public async Task<List<WorkshopResponse>> GetAllAsync([FromQuery] string? venueName, [FromQuery] DateOnly? date)
        => await _workshopMapper.GetAllAsync(venueName, date);

    [HttpGet("paged")]
    public async Task<PaginatedResponse<WorkshopResponse>> GetAllPaged([FromQuery] PaginatedRequest request)
        => await _workshopMapper.GetAllPaginatedAsync(request);

    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] WorkshopRequest request)
        => Ok(await _workshopMapper.InsertAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] WorkshopRequest request)
        => Ok(await _workshopMapper.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await _workshopMapper.DeleteAsync(id));
}
