using ExamsApplication.Domain.Dto;
using ExamsApplication.Domain.Models;
using ExamsApplication.Web.Request;
using ExamsApplication.Web.Response;

namespace ExamsApplication.Web.Extensions;

public static class ExamSlotMappingExtenstions
{
    public static ExamSlotResponse ToResponse(this ExamSlot e)
    {
        return new ExamSlotResponse(
            ExamSlotId: e.Id,
            StartTime: e.StartTime,
            EndTime: e.EndTime,
            CourseId: e.CourseId,
            CourseName: e.Course.Title
        );
    }

    public static ExamSlotWithExamAttemptsResponse ExamSlotWithExamAttemptsResponse(this ExamSlot e)
    {
        return new ExamSlotWithExamAttemptsResponse(
            ExamSlotId: e.Id,
            StartTime: e.StartTime,
            EndTime: e.EndTime,
            CourseId: e.CourseId,
            CourseName: e.Course.Title,
            Responses: e.ExamAttempts.ToList().ToResponse()
        );
    }

    public static List<ExamSlotResponse> ToResponse(this List<ExamSlot> examSlots)
    {
        return examSlots.Select(s => s.ToResponse()).ToList();
    }

    public static ExamSlotDto CreateOrUpdateExamSlotRequest(this ExamSlotRequest request)
    {
        return new ExamSlotDto()
        {
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            CourseId = request.CourseId
        };
    }

    public static PaginatedResponse<ExamSlotWithExamAttemptsResponse> ToPaginatedResponse(
        this PaginatedResult<ExamSlot> result)
    {
        return new PaginatedResponse<ExamSlotWithExamAttemptsResponse>()
        {
            Items = result.Items.Select(r => r.ExamSlotWithExamAttemptsResponse()).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}