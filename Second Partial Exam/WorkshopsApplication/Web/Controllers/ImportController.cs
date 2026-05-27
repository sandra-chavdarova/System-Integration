using Domain.Models;
using Domain.Enums;
using Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IExcelImportService _importService;
    private readonly IWorkshopService _workshopService;

    public ImportController(IExcelImportService importService, IWorkshopService workshopService)
    {
        _importService = importService;
        _workshopService = workshopService;
    }

    [HttpPost("workshops")]
    public async Task<IActionResult> ImportWorkshops(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded");
        if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            return BadRequest("Only .xlsx supported");
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File exceeds 5 MB");

        using var stream = file.OpenReadStream();
        var result = await _importService.ImportWorkshopsAsync(stream);

        if (result.HasErrors)
        {
            return Ok(new
            {
                success = false, totalRows = result.TotalRows,
                successCount = result.SuccessfulRecords.Count,
                errorCount = result.Errors.Count, errors = result.Errors
            });
        }

        foreach (var dto in result.SuccessfulRecords)
        {
            await _workshopService.CreateAsync(dto.Title, dto.StartDate, dto.EndDate, dto.VenueId);
        }

        return Ok(new { success = true, totalRows = result.TotalRows, createdCount = result.SuccessfulRecords.Count });
    }
}
