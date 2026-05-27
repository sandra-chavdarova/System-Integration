namespace Service.Interface;

public interface IWorkshopsApiClient<T> where T : class
{
    Task<T> GetAllWorkshopsModifiedSinceAsync(DateTime dateLastModified);
}
