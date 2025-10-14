using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;

namespace ElectroKart_Api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> CreateProductAsync(CreateProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                CategoryId = productDto.CategoryId
            };

            return await _productRepository.CreateAsync(product);
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<List<ProductDto>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            return products.Select(MapToDto).ToList();
        }

        // --- New search/filter method ---
        public async Task<List<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto)
        {
            var products = await _productRepository.SearchProductsAsync(searchDto);
            return products.Select(MapToDto).ToList();
        }

        // --- Helper method to map Product to ProductDto ---
        private ProductDto MapToDto(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryName = p.Category?.Name ?? "N/A"
            };
        }
    }
}
