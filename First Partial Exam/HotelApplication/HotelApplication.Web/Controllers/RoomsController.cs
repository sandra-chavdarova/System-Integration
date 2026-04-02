using HotelApplication.Web.Mappers;
using HotelApplication.Web.Request;
using HotelApplication.Web.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly RoomMapper _mapper;

    public RoomsController(RoomMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mapper.GetByIdNotNullAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomResponse>>> GetAll([FromQuery] int status)
    {
        var result = await _mapper.GetAllAsync(status);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PaginatedResponse<RoomResponse>>> GetAllPaged([FromQuery] PaginatedRequest request)
    {
        var result = await _mapper.GetAllPagedAsync(request);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomResponse>> Create([FromBody] RoomRequest request)
    {
        var result = await _mapper.InsertAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomResponse>> Update([FromRoute] Guid id, [FromBody] RoomRequest request)
    {
        var result = await _mapper.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<RoomResponse>> Delete([FromRoute] Guid id)
    {
        var result = await _mapper.DeleteAsync(id);
        return Ok(result);
    }
}