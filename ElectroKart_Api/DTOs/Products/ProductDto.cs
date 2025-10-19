namespace ElectroKart_Api.DTOs.Products
{
    /// <summary>
    /// DTO representing a product returned from the API.
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = "N/A";

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
