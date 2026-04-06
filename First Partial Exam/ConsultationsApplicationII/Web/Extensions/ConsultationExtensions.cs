using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class ConsultationExtensions
{
    public static ConsultationBasicResponse ToBasicResponse(this Consultation c)
    {
        return new ConsultationBasicResponse(
            Id: c.Id,
            RoomId: c.RoomId,
            Start: c.StartTime,
            End: c.EndTime
        );
    }

    public static ConsultationResponse ToResponse(this Consultation c)
    {
        return new ConsultationResponse(
            Id: c.Id,
            Date: DateOnly.FromDateTime(c.StartTime),
            RoomId: c.RoomId,
            RegisteredStudents: c.RegisteredStudents,
            Attendances: c.Attendances.Select(x => x.ToBasicResponse()).ToList()
        );
    }
}