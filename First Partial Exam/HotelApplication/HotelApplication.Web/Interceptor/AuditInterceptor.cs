using HotelApplication.Domain.Common;
using HotelApplication.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HotelApplication.Web.Interceptor;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context!;
        var entries = context.ChangeTracker
            .Entries<BaseAuditableEntity<string>>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var user = _currentUser.GetUserId() ?? "system";

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = user;
                entry.Entity.DateCreated = now;
                entry.Entity.LastModifiedById = user;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedById = user;
                entry.Entity.DateLastModified = now;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}
