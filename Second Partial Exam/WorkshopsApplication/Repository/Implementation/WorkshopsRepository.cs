using Domain.Models;
using EFCore.BulkExtensions;
using Repository.Interface;

namespace Repository.Implementation;

public class WorkshopsRepository : Repository<Workshop>, IWorkshopsRepository
{
    public WorkshopsRepository(ApplicationDbContext context) : base(context) { }

    public async Task BulkInsertOrUpdateAsync(List<Workshop> workshops)
    {
        await _context.BulkInsertOrUpdateAsync(workshops);
    }
}
