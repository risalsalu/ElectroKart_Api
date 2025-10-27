using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ElectroKart_Api.DTOs.Products
{

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000.")]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImagePublicId { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
