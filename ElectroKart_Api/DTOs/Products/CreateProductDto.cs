using System.ComponentModel.DataAnnotations;

namespace ElectroKart_Api.DTOs.Products
{
    public class CreateProductDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required, Range(0.01, 100000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
