namespace WebAppCosmosDBDEMO.Services
{
    public interface IProductService<T> where T : class
    {
        Task<T?> GetProductByIdAsync(string id, string partitionKey);
        Task<IEnumerable<T>> QueryProductAsync(string sqlQuery, 
                                                Dictionary<string, object> parameters, 
                                                string? partitionKey = null);
        Task<T> UpsertProductAsync(T entity, string partitionKey);
        Task DeleteProductAsync(string id, string partitionKey);

        Task<IEnumerable<T>> GetMaxPricePerCategory(string categoryId);
    }
}
