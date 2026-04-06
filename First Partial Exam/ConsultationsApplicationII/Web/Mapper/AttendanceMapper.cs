using Domain.Dto;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class AttendanceMapper
{
    private readonly IAttendanceService _service;
    private readonly IFileUploadService _fileUploadService;

    public AttendanceMapper(IAttendanceService service, IFileUploadService fileUploadService)
    {
        _service = service;
        _fileUploadService = fileUploadService;
    }

    public async Task<AttendanceResponse> RegisterAsync(AttendanceRequest request)
    {
        var dto = new AttendanceDto()
        {
            Comment = request.Comment,
            ConsultationId = request.ConsultationId,
            RoomId = request.RoomId,
            UserId = request.UserId
        };
        var result = await _service.CreateAsync(dto);
        return result.ToResponse();
    }

    public async Task<AttendanceBasicResponse> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteByIdAsync(id);
        return result.ToBasicResponse();
    }

    public async Task<List<AttendanceResponse>> GetAllByConsultationIdAsync(Guid id)
    {
        var result = await _service.GetAllByConsultationIdAsync(id);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task MarkAsAbsentAsync(Guid id)
    {
        await _service.MarkAsAbsentByIdAsync(id);
    }

    public async Task<AttendanceResponse> UploadReasonByIdInFileSystemAsync(Guid id, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var path = await _fileUploadService.UploadFileAsync(
            ms.ToArray(),
            file.FileName
        );

        var result = await _service.UpdateReasonPathByIdAsync(id, path);

        return result.ToResponse();
    }
}