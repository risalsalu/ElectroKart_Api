namespace ElectroKart_Api.DTOs.Products
{

    public class ProductSearchDto
    {
        public string? Name { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int? CategoryId { get; set; }
    }
}
