using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class WorkshopExtensions
{
    public static WorkshopResponse ToResponse(this Workshop w) =>
        new(w.Id, DateOnly.FromDateTime(w.StartTime), w.Title, w.VenueId, w.RegisteredParticipants,
            w.Enrollments.Select(e => e.ToBasicResponse()).ToList());

    public static WorkshopBasicResponse ToBasicResponse(this Workshop w) =>
        new(w.Id, w.VenueId, w.Title, w.StartTime, w.EndTime);
}
