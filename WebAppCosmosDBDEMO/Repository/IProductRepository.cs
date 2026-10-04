using WebAppCosmosDBDEMO.Models;

namespace WebAppCosmosDBDEMO.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryMaxPrice(string category, decimal maxPrice);
    }
}
