using Domain.Models;

namespace Repository.Interface;

public interface IWorkshopsRepository
{
    Task BulkInsertOrUpdateAsync(List<Workshop> workshops);
}
