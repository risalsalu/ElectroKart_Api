using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services.Products
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(CreateProductDto productDto);
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<List<ProductDto>> GetProductsByCategoryIdAsync(int categoryId);
        Task<List<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto);
    }
}
