using Domain.Dto;
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class EnrollmentMapper
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IFileUploadService _fileUploadService;

    public EnrollmentMapper(IEnrollmentService enrollmentService, IFileUploadService fileUploadService)
    {
        _enrollmentService = enrollmentService;
        _fileUploadService = fileUploadService;
    }

    public async Task<EnrollmentResponse> RegisterAsync(EnrollmentRequest request)
    {
        var result = await _enrollmentService.CreateAsync(new EnrollmentDto
        {
            Notes = request.Notes, WorkshopId = request.WorkshopId,
            UserId = request.UserId, VenueId = request.VenueId,
        });
        return result.ToResponse();
    }

    public async Task DeleteAsync(Guid id) => await _enrollmentService.DeleteByIdAsync(id);

    public async Task<List<EnrollmentResponse>> GetAllByWorkshopIdAsync(Guid workshopId)
    {
        var result = await _enrollmentService.GetAllByWorkshopIdAsync(workshopId);
        return result.Select(x => x.ToResponse()).ToList();
    }

    public async Task MarkAsAbsentAsync(Guid id) => await _enrollmentService.MarkAsAbsentByIdAsync(id);

    public async Task<EnrollmentResponse> UploadCancellationAsync(Guid id, IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var path = await _fileUploadService.UploadFileAsync(ms.ToArray(), file.FileName);
        var result = await _enrollmentService.UpdateCancellationPathByIdAsync(id, path);
        return result.ToResponse();
    }
}
