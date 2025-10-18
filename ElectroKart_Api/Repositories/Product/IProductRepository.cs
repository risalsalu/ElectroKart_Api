using ElectroKart_Api.Models;
using ElectroKart_Api.DTOs.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
        Task<List<Product>> GetByCategoryIdAsync(int categoryId, int pageNumber = 1, int pageSize = 20);
        Task<List<Product>> SearchProductsAsync(ProductSearchDto searchDto, int pageNumber = 1, int pageSize = 20);
    }
}
