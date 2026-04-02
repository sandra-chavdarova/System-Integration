using CoursesApplication.Domain.Dto;
using CoursesApplication.Web.Response;

namespace CoursesApplication.Web.Extensions;

public static class PaginatedResponseExtension
{
    public static PaginatedResponse<TResult> ToPaginatedResponse<T, TResult>(
        this PaginatedResult<T> result,
        Func<T, TResult> mappingFunction)
    {
        return new PaginatedResponse<TResult>
        {
            Items = result.Items.Select(mappingFunction).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }

}