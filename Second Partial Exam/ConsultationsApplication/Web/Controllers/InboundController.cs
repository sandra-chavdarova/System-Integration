using System.Text.Json;
using Domain.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Service.Implementation;

namespace Web.Controllers;

[ApiController]
[EnableRateLimiting("external-api")]
[Route("api/external/attendance")]
public class InboundController : ControllerBase
{
    private readonly IInboundEventEntryService _entryService;

    public InboundController(IInboundEventEntryService entryService)
    {
        _entryService = entryService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> ReceiveEvent([FromBody] InboundEventRequest request)
    {
        var payload = JsonSerializer.Serialize(request);

        var entry = await _entryService.CreateAsync(payload);

        return Accepted(new
        {
            Id = entry.Id,
            Status = "Pending",
        });
    }

    [HttpGet("register/{id}/status")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken ct)
    {
        try
        {
            var entry = await _entryService.GetByIdNotNull(id);
            return Ok(new
            {
                Id = entry.Id,
                Status = entry.Status.ToString(),
                Error = entry.ErrorMessage
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound($"No entry found with id {id}");
        }
    }
}