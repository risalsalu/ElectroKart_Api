using ElectroKart_Api.Models;

namespace ElectroKart_Api.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId);
    }
}