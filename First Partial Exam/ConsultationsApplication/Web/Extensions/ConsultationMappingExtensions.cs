using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class ConsultationMappingExtensions
{
    public static ConsultationResponse ToResponse(this Consultation c)
    {
        return new ConsultationResponse(
            StartTime: c.StartTime,
            EndTime: c.EndTime,
            RoomName: c.Room.Name
        );
    }

    public static ConsultationWithAttendancesResponse ToConsultationWithAttendancesResponse(this Consultation c)
    {
        return new ConsultationWithAttendancesResponse(
            StartTime: c.StartTime,
            EndTime: c.EndTime,
            RoomName: c.Room.Name,
            AttendanceResponses: c.Attendances.ToList().ToResponse()
        );
    }

    public static List<ConsultationResponse> ToResponse(this List<Consultation> consultations)
    {
        return consultations.Select(c => c.ToResponse()).ToList();
    }

    public static ConsultationDto CreateOrUpdateConsultationRequest(this ConsultationRequest request)
    {
        return new ConsultationDto()
        {
            EndTime = request.EndTime,
            RoomId = request.RoomId,
            StartTime = request.StartTime
        };
    }

    public static PaginatedResponse<ConsultationWithAttendancesResponse> ToPaginatedResponse(
        this PaginatedResult<Consultation> result)
    {
        return new PaginatedResponse<ConsultationWithAttendancesResponse>()
        {
            Items = result.Items.Select(x => x.ToConsultationWithAttendancesResponse()).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages
        };
    }
}