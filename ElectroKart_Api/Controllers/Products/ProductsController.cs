using ElectroKart_Api.DTOs.Products;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services;
using ElectroKart_Api.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductsController(IProductService productService, ICloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile);
                productDto.ImageUrl = uploadResult.SecureUrl.ToString();
                productDto.ImagePublicId = uploadResult.PublicId;
            }

            var createdProduct = await _productService.CreateProductAsync(productDto);
            var productToReturn = await _productService.GetProductByIdAsync(createdProduct.Id);

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, productToReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchDto searchDto)
        {
            var products = await _productService.SearchProductsAsync(searchDto);
            return Ok(products);
        }
    }
}
