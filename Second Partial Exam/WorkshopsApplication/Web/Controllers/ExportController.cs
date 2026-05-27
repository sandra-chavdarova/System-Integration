using Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExcelExportService _excelExportService;
    public ExportController(IExcelExportService excelExportService) { _excelExportService = excelExportService; }

    [HttpGet("enrollments/{workshopId}")]
    public async Task<IActionResult> ExportAsync(Guid workshopId)
    {
        var bytes = await _excelExportService.ExportEnrollmentsToExcelAsync(workshopId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"enrollments-{workshopId:N}.xlsx");
    }
}
