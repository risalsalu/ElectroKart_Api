using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ElectroKart_Api.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        /// <summary>
        /// Upload a new image to Cloudinary.
        /// </summary>
        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        /// <summary>
        /// Delete an image from Cloudinary using its public ID.
        /// </summary>
        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("PublicId cannot be null or empty.", nameof(publicId));

            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        /// <summary>
        /// Update product image: delete old image if exists, then upload new image.
        /// </summary>
        public async Task<(ImageUploadResult uploadResult, DeletionResult? deletionResult)> ReplaceImageAsync(IFormFile newFile, string? oldPublicId)
        {
            DeletionResult? deletionResult = null;

            // Delete old image if exists
            if (!string.IsNullOrEmpty(oldPublicId))
            {
                deletionResult = await DeleteImageAsync(oldPublicId);
            }

            // Upload new image
            var uploadResult = await UploadImageAsync(newFile);
            return (uploadResult, deletionResult);
        }
    }
}
