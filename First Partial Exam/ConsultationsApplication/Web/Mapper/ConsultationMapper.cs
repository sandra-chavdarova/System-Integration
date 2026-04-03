using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ConsultationMapper
{
    private readonly IConsultationService _service;

    public ConsultationMapper(IConsultationService service)
    {
        _service = service;
    }

    public async Task<ConsultationResponse> GetById(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<List<ConsultationResponse>> GetAll(string? dateAfter)
    {
        var result = await _service.GetAllAsync(dateAfter);
        return result.ToResponse();
    }

    public async Task<ConsultationResponse> Create(ConsultationRequest request)
    {
        var dto = request.CreateOrUpdateConsultationRequest();
        var result = await _service.CreateAsync(dto);
        return result.ToResponse();
    }

    public async Task<ConsultationResponse> Update(Guid id, ConsultationRequest request)
    {
        var dto = request.CreateOrUpdateConsultationRequest();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }

    public async Task<ConsultationResponse> Delete(Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);
        return result.ToResponse();
    }

    public async Task<PaginatedResponse<ConsultationResponse>> GetPaged(PaginatedRequest request)
    {
        var result = await _service.GetPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(r => r.ToResponse());
    }
}