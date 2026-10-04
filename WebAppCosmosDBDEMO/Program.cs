using Microsoft.Azure.Cosmos;
using WebAppCosmosDBDEMO.Models;
using WebAppCosmosDBDEMO.Repository;
using WebAppCosmosDBDEMO.Services;

namespace WebAppCosmosDBDEMO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSingleton<CosmosClient>(sp =>
            {
                string connectionString = builder.Configuration.GetConnectionString("CosmosDb")
                    ?? throw new InvalidOperationException("CosmosDb connection string is missing.");

                return new CosmosClient(connectionString, new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                });
            });

            builder.Services.AddSingleton<IProductRepository>(sp =>
            {
                CosmosClient client = sp.GetRequiredService<CosmosClient>();
                Container container = client.GetContainer("AHDatabase", "Products");
                return new ProductRepository(container);
            });
            builder.Services.AddScoped<IProductService<Product>, ProductService>();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

