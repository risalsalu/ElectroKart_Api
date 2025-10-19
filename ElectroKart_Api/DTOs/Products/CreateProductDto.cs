using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ElectroKart_Api.DTOs.Products
{
    /// <summary>
    /// DTO for creating a new product.
    /// Supports optional image upload.
    /// </summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000.")]
        public decimal Price { get; set; }

        /// <summary>
        /// Optional product image file.
        /// </summary>
        public IFormFile? Image { get; set; }

        /// <summary>
        /// Cloudinary URL of uploaded image (set by backend).
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Cloudinary public ID of uploaded image (set by backend).
        /// </summary>
        public string? ImagePublicId { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public int CategoryId { get; set; }
    }
}
