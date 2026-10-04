using Microsoft.Azure.Cosmos;
using WebAppCosmosDBDEMO.Models;

namespace WebAppCosmosDBDEMO.Repository
{
    public class ProductRepository : CosmosRepository<Product>, IProductRepository
    {
        public ProductRepository
            (Container container)
        : base(container)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryMaxPrice(string category, decimal maxPrice)
        {
            string query = "SELECT * FROM p WHERE p.category = @category AND p.price <= @maxPrice";
            var parameters = new Dictionary<string, object>
        {
            { "@category", category },
            { "@maxPrice", maxPrice }
        };

            return await QueryAsync(query, parameters, partitionKey: category);
        }
    }
}
