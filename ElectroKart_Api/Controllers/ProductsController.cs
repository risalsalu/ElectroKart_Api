using ElectroKart_Api.Attributes;
using ElectroKart_Api.Models;
using ElectroKart_Api.Services; // <-- CHANGE THIS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectroKart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // --- CHANGE THIS ---
        private readonly IProductService _productService;

        // --- CHANGE THIS ---
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // POST /api/products
        [HttpPost]
        [Authorize]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // --- CHANGE THIS ---
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        // GET /api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            // --- CHANGE THIS ---
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET /api/products/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            // --- CHANGE THIS ---
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET /api/products/category/2
        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            // --- CHANGE THIS ---
            var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(products);
        }
    }
}