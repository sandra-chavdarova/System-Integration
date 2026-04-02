using ExamsApplication.Domain.Models;
using ExamsApplication.Web.Request;

namespace ExamsApplication.Web.Extensions;

public static class ExamAttemptMappingExtensions
{
    public static ExamAttemptResponse ToResponse(this ExamAttempt e)
    {
        return new ExamAttemptResponse(
            ExamAttemptId: e.Id,
            UserId: e.UserId,
            UserName: string.Concat(str0: e.User.FirstName, str1: " ", str2: e.User.LastName),
            Grade: e.Grade
        );
    }

    public static List<ExamAttemptResponse> ToResponse(this List<ExamAttempt> attempts)
    {
        return attempts.Select(a => a.ToResponse()).ToList();
    }
}