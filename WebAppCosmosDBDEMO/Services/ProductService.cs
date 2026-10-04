using WebAppCosmosDBDEMO.Models;
using WebAppCosmosDBDEMO.Repository;

namespace WebAppCosmosDBDEMO.Services
{
    public class ProductService : IProductService<Product>
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public Task DeleteProductAsync(string id, string partitionKey)
        {
            return _repository.DeleteAsync(id, partitionKey);
        }

        public Task<Product?> GetProductByIdAsync(string id, string partitionKey)
        {
            return _repository.GetByIdAsync(partitionKey, id);
        }

        public async Task <IEnumerable<Product>> QueryProductAsync(string sqlQuery, Dictionary<string, object> parameters, string? partitionKey = null)
        {
            return await _repository.QueryAsync(sqlQuery, parameters, partitionKey);
        }

        public Task<Product> UpsertProductAsync(Product entity, string partitionKey)
        {
            return _repository.UpsertAsync(entity, partitionKey);
        }

        Task<IEnumerable<Product>> IProductService<Product>.GetMaxPricePerCategory(string categoryId)
        {
            return _repository.GetByCategoryMaxPrice(categoryId, 1000m);
        }
    }
}
