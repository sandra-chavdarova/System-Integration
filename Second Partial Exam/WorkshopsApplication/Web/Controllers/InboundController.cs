// TODO: Implement InboundController
// - Route: /api/external/enrollment
// - Add [EnableRateLimiting("external-api")] attribute
// - POST register: serialize request body, create InboundEventEntry via service, return 202 Accepted
// - GET register/{id}/status: get entry by id, return status object

using System.Text.Json;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/external/enrollment")]
[EnableRateLimiting("external-api")]
public class ExternalEventsController : ControllerBase
{
    private readonly IInboundEventEntryService _service;

    public ExternalEventsController(IInboundEventEntryService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> ReceiveEvent([FromBody] EnrollmentRequestDto request)
    {
        var payload = JsonSerializer.Serialize(request);

        var entry = await _service.CreateAsync(payload);

        return Accepted(new
        {
            id = entry.Id,
            status = "Pending",
        });
    }

    [HttpGet("register/{id}/status")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken ct)
    {
        var entry = await _service.GetByIdNotNullAsync(id);

        return Ok(new
        {
            status = entry.Status.ToString(),
            id = entry.Id,
            error = entry.ErrorMessage
        });
    }
}