using Microsoft.AspNetCore.Components.Forms;

namespace MEMS.Admin.Services;

public class ImageUploadService(IWebHostEnvironment env)
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public async Task<string> SaveImageAsync(IBrowserFile file, string subfolder)
    {
        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Only .jpg, .jpeg, .png, and .webp images are allowed.");

        if (file.Size > MaxFileSize)
            throw new InvalidOperationException("Image size cannot exceed 5 MB.");

        var uploadsFolder = Path.Combine(env.WebRootPath, "images", subfolder);
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.OpenReadStream(MaxFileSize).CopyToAsync(stream);

        return $"/images/{subfolder}/{fileName}";
    }
}