// TODO: Implement ExcelImportService
// - Implement IExcelImportService
// - Inject IRepository<Venue> (to look up venues by name)
// - ImportWorkshopsAsync(Stream):
//   1. Open XLWorkbook from stream
//   2. Read header row, map column names (lowercase) to indices
//   3. Validate required headers: "title", "startdate", "enddate", "venue"
//   4. For each data row:
//      - Validate title not empty
//      - Validate startdate/enddate are valid DateTime
//      - Validate enddate > startdate
//      - Validate venue exists in database
//   5. Return ImportResult<WorkshopImportDto> with successful records and errors
using Service.Interface;

namespace Service.Implementation;

public class ExcelImportService : IExcelImportService
{
    // TODO: Implement
    public Task<Domain.Dto.ImportResult<Domain.Dto.WorkshopImportDto>> ImportWorkshopsAsync(Stream fileStream)
        => throw new NotImplementedException();
}
