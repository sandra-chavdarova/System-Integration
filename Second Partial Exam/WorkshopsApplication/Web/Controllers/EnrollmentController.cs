using Microsoft.AspNetCore.Mvc;
using Web.Mapper;
using Web.Request;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly EnrollmentMapper _enrollmentMapper;
    public EnrollmentController(EnrollmentMapper enrollmentMapper) { _enrollmentMapper = enrollmentMapper; }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] EnrollmentRequest request)
        => Ok(await _enrollmentMapper.RegisterAsync(request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id) { await _enrollmentMapper.DeleteAsync(id); return Ok(); }

    [HttpGet("workshop/{id}")]
    public async Task<IActionResult> GetByWorkshopAsync(Guid id)
        => Ok(await _enrollmentMapper.GetAllByWorkshopIdAsync(id));

    [HttpPatch("{id}/mark-as-absent")]
    public async Task<IActionResult> MarkAsAbsentAsync(Guid id) { await _enrollmentMapper.MarkAsAbsentAsync(id); return Ok(); }

    [HttpPost("{id}/cancellation-reason")]
    public async Task<IActionResult> UploadCancellationAsync([FromRoute] Guid id, [FromForm] IFormFile file)
        => Ok(await _enrollmentMapper.UploadCancellationAsync(id, file));
}
