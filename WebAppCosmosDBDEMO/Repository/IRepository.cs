using Microsoft.Azure.Cosmos;

namespace WebAppCosmosDBDEMO.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id, string partitionKey);
        Task<IEnumerable<T>> QueryAsync(string sqlQuery, Dictionary<string, object> parameters, string? partitionKey = null);
        Task<T> UpsertAsync(T entity, string partitionKey);
        Task DeleteAsync(string id, string partitionKey);

        //YUCK
        FeedIterator<TEntity> GetItemQueryIterator<TEntity>(QueryDefinition queryDefinition, QueryRequestOptions requestOptions = null);
    }
}
