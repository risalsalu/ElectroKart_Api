using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;

namespace ElectroKart_Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            // Business logic for creating a product would go here
            return await _productRepository.CreateAsync(product);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }
    }
}