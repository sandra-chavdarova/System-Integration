// TODO: Implement ExcelExportService
// - Implement IExcelExportService
// - Inject IEnrollmentService
// - ExportEnrollmentsToExcelAsync(workshopId):
//   1. Get all enrollments by workshopId
//   2. Create XLWorkbook with sheet "Enrollments"
//   3. Headers: "Enrollment ID", "User", "Status", "Notes", "Venue"
//   4. Style header row (bold, background color #4F46E5, white font, centered)
//   5. Fill data rows
//   6. Auto-fit columns, add auto-filter, freeze header row
//   7. Return byte[] from MemoryStream
using Service.Interface;

namespace Service.Implementation;

public class ExcelExportService : IExcelExportService
{
    // TODO: Implement
    public Task<byte[]> ExportEnrollmentsToExcelAsync(Guid workshopId)
        => throw new NotImplementedException();
}
