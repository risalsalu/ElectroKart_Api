using ElectroKart_Api.Models;
using ElectroKart_Api.DTOs.Products;

namespace ElectroKart_Api.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByCategoryIdAsync(int categoryId);

        // New method for search/filter
        Task<List<Product>> SearchProductsAsync(ProductSearchDto searchDto);
    }
}
