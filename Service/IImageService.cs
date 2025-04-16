namespace UserGuard_API.Service
{
    public interface IImageService
    {

        Task<string?> SaveImageAsync(IFormFile file, string folderName);
    }
}
