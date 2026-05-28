using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IConsultationsApiClient<T> where T : class
{
    Task<ExternalConsultationsDto> GetAllConsultationsModifiedSinceAsync(DateTime? lastModified);
}