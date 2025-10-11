using ElectroKart_Api.Models;

namespace ElectroKart_Api.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByCategoryIdAsync(int categoryId);
    }
}