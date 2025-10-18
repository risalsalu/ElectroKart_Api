using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Helpers;
using ElectroKart_Api.Models;
using ElectroKart_Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                ImagePublicId = dto.ImagePublicId,
                CategoryId = dto.CategoryId
            };

            var created = await _productRepo.CreateAsync(product);
            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(created));
        }

        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var existing = await _productRepo.GetByIdAsync(dto.Id);
            if (existing == null)
                return ApiResponse<ProductDto>.FailureResponse("Product not found");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CategoryId = dto.CategoryId;
            existing.ImageUrl = dto.ImageUrl;
            existing.ImagePublicId = dto.ImagePublicId;
            existing.IsActive = dto.IsActive;

            var updated = await _productRepo.UpdateAsync(existing);
            return ApiResponse<ProductDto>.SuccessResponse(MapToDto(updated!));
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.FailureResponse("Product not found");

            var deleted = await _productRepo.DeleteAsync(existing);
            return ApiResponse<bool>.SuccessResponse(deleted);
        }

        public async Task<ApiResponse<ProductDto?>> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return ApiResponse<ProductDto?>.FailureResponse("Product not found");

            return ApiResponse<ProductDto?>.SuccessResponse(MapToDto(product));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepo.GetByCategoryIdAsync(categoryId);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(ProductSearchDto dto)
        {
            var products = await _productRepo.SearchProductsAsync(dto);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products.Select(MapToDto));
        }

        private ProductDto MapToDto(Product p)
        {
            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "N/A",
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive
            };
        }
    }
}
