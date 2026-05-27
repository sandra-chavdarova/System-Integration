using Domain.Dto.Email;
using Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailQueue _emailQueue;
    private readonly IExcelExportService _excelExportService;

    public EmailController(IEmailQueue emailQueue, IExcelExportService excelExportService)
    {
        _emailQueue = emailQueue;
        _excelExportService = excelExportService;
    }

    [HttpPost("send-enrollment-report")]
    public async Task<IActionResult> SendEnrollmentReport([FromBody] SendReportRequest request)
    {
        var excelBytes = await _excelExportService.ExportEnrollmentsToExcelAsync(request.WorkshopId);

        var message = new EmailMessage
        {
            To = request.RecipientEmail,
            Subject = $"Enrollment Report - Workshop {request.WorkshopId}",
            HtmlBody = "<h1>Enrollment Report</h1><p>Please find the attached enrollment report.</p>",
            PlainText = "Enrollment Report - see attachment.",
            Attachments = new List<EmailAttachment>
            {
                new()
                {
                    FileName = $"enrollments-{request.WorkshopId:N}.xlsx",
                    Content = excelBytes,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                }
            }
        };

        await _emailQueue.EnqueueAsync(message);
        return Accepted(new { message = "Report queued for delivery" });
    }
}

public record SendReportRequest(Guid WorkshopId, string RecipientEmail);
