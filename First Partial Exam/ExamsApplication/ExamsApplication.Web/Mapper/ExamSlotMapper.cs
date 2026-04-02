using ExamsApplication.Service.Interface;
using ExamsApplication.Web.Extensions;
using ExamsApplication.Web.Request;
using ExamsApplication.Web.Response;

namespace ExamsApplication.Web.Mapper;

public class ExamSlotMapper
{
    private readonly IExamSlotService _service;

    public ExamSlotMapper(IExamSlotService service)
    {
        _service = service;
    }

    public async Task<ExamSlotResponse> GetByIdNotNull(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<List<ExamSlotResponse>> GetAll(string? sessionType)
    {
        var result = await _service.GetAllAsync(sessionType);
        return result.ToResponse();
    }

    public async Task<ExamSlotResponse> Create(ExamSlotRequest request)
    {
        var dto = request.CreateOrUpdateExamSlotRequest();
        var result = await _service.CreateAsync(dto);
        return result.ToResponse();
    }

    public async Task<ExamSlotResponse> Update(Guid id, ExamSlotRequest request)
    {
        var dto = request.CreateOrUpdateExamSlotRequest();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }

    public async Task<ExamSlotResponse> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<ExamSlotResponse>> GetAllPaged(PaginatedRequest request)
    {
        var result = await _service.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(r => r.ToResponse());
    }
}