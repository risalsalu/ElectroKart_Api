namespace ElectroKart_Api.DTOs.Products
{
    public class ProductSearchDto
    {
        public string? Query { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
