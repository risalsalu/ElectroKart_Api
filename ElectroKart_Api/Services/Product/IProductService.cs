using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectroKart_Api.Services.Products
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto productDto);
        Task<ApiResponse<ProductDto>> UpdateProductAsync(UpdateProductDto productDto);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<ApiResponse<ProductDto?>> GetProductByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryIdAsync(int categoryId);
        Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(ProductSearchDto searchDto);
    }
}
