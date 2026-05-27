namespace Service.Interface;

public interface IExcelExportService
{
    Task<byte[]> ExportEnrollmentsToExcelAsync(Guid workshopId);
}
