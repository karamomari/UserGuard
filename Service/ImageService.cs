namespace UserGuard_API.Service
{
    public class ImageService : IImageService
    {
        public async Task<string?> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", folderName);
            Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{folderName}/{fileName}";
        }
    }

}
