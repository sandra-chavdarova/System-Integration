using Domain.Dto;

namespace Service.Interface;

public interface IExcelImportService
{
    Task<ImportResult<WorkshopImportDto>> ImportWorkshopsAsync(Stream fileStream);
}
