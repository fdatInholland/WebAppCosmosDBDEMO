using Microsoft.Azure.Cosmos;
using WebAppCosmosDBDEMO.Utils;

namespace WebAppCosmosDBDEMO.Repository
{
    public class CosmosRepository<T> : IRepository<T> where T : class
    {
        private readonly Container _container;

        public CosmosRepository(Container container)
        {
            _container = container;
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task<T?> GetByIdAsync(string id, string partitionKey)
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));

            return response.Resource;
        }

        public async Task<IEnumerable<T>> QueryAsync(string sqlQuery, Dictionary<string, object> parameters, string? partitionKey = null)
        {
            QueryDefinition queryDefinition = new(sqlQuery);
            foreach (var param in parameters)
            {
                string paramKey = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                paramKey = paramKey.ToLower();

                object paramValue = UnwrapJsonValue(param.Value);

                if (paramKey.Contains("price") || paramKey.Contains("quantity"))
                {
                    paramValue = ParameterFormattingExtensions.ToDecimal(param.Value);
                }
                queryDefinition = queryDefinition.WithParameter(paramKey, paramValue);
            }

            List<T> results = new();
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = string.IsNullOrEmpty(partitionKey)
                        ? null
                        : new PartitionKey(partitionKey)
            };

            using FeedIterator<T> feedIterator = _container.GetItemQueryIterator<T>(
                queryDefinition,
                requestOptions: requestOptions
            );

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<T> response = await feedIterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<T> UpsertAsync(T entity, string partitionKey)
        {
            ItemResponse<T> response = await _container.UpsertItemAsync(entity, new PartitionKey(partitionKey));
            return response.Resource;
        }

        FeedIterator<TEntity> IRepository<T>.GetItemQueryIterator<TEntity>(QueryDefinition queryDefinition, QueryRequestOptions requestOptions)
        {
            return _container.GetItemQueryIterator<TEntity>(queryDefinition, requestOptions: requestOptions);
        }

        private static object UnwrapJsonValue(object value)
        {
            if (value is System.Text.Json.JsonElement element)
            {
                return element.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => element.GetString() ?? string.Empty,
                    System.Text.Json.JsonValueKind.Number => element.TryGetInt64(out long l) ? l : element.GetDouble(),
                    System.Text.Json.JsonValueKind.True => true,
                    System.Text.Json.JsonValueKind.False => false,
                    System.Text.Json.JsonValueKind.Null => null!,
                    _ => element.GetRawText()
                };
            }
            return value;
        }
    }
}
