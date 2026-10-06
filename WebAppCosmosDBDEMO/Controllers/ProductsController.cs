using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using WebAppCosmosDBDEMO.DTO;
using WebAppCosmosDBDEMO.Models;
using WebAppCosmosDBDEMO.Services;

namespace WebAppCosmosDBDEMO.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
     private readonly IProductService<Product> _productService;
        public ProductsController(IProductService<Product> productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            Product product = new Product{
                Id = Guid.NewGuid().ToString(),
                Category = dto.Category,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            // Partition Key is required for Cosmos DB operations
            PartitionKey partitionKey = new(product.Category);

            Product response = await _productService.UpsertProductAsync(product, partitionKey: product.Category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id, category = product.Category },
                response
            );
        }

        //Postman http://localhost:5139/products/kitchen/be1ab58e-a09c-4744-985a-34511e479831
        [HttpGet("{category}/{id}")]
        public async Task<IActionResult> GetById(string category, string id)
        {
            // "be1ab58e-a09c-4744-985a-34511e479831"

            Product foundproduct = await _productService.GetProductByIdAsync(category, id);

            if (foundproduct is null)
            {
                return NotFound("Product Not found");
            }
            return Ok(foundproduct);
        }

        //Postman http://localhost:5139/products/query?category=electronics&maxPrice=150
        [HttpGet("query")]
        public async Task<IActionResult> Query([FromBody] ProductQueryRequestDTO productQueryRequestDTO, 
                                               [FromQuery] string? category, 
                                               [FromQuery] decimal maxPrice = 1000m)
        {
            //Guard Clause
            if (string.IsNullOrWhiteSpace(category) || (string.IsNullOrEmpty(productQueryRequestDTO.SqlQuery)))
            {
                return BadRequest("Insufficient required parameters.");
            }


            IEnumerable<Product> products = await _productService.QueryProductAsync(productQueryRequestDTO.SqlQuery, 
                                                                                    productQueryRequestDTO.Parameters ?? 
                                                                                    new Dictionary<string, object>(),
                                                                                    category);

            return Ok(products);
        }

        //Postman http://localhost:5139/products/books/3b730d87-33a2-47bf-89e7-bbd7cdb3f1f4
        [HttpDelete("{category}/{id}")]
        public async Task<IActionResult> Delete(string category, string id)
        {
            await _productService.DeleteProductAsync(id, category);
            return NoContent();
        }
    }
}
