using Microsoft.Azure.Cosmos;
using Translator2.Models;

namespace Translator2.Services
{
    public class ProductCosmosService
    {
        private readonly Container _container;

        public ProductCosmosService(IConfiguration config)
        {
            var client = new CosmosClient(config["Cosmos:ConnectionString"]);

            var db = client.GetDatabase(config["Cosmos:DatabaseName"]);

            _container = db.GetContainer("Products");
        }

        public async Task<List<Product>> GetProductsByCategory(string categoryId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.categoryId = @id")
                .WithParameter("@id", categoryId);

            var iterator = _container.GetItemQueryIterator<Product>(query);

            var result = new List<Product>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<List<(string Id, string Name)>> GetCategories()
        {
            var query = new QueryDefinition(
                "SELECT DISTINCT c.categoryId, c.categoryName FROM c");

            var iterator = _container.GetItemQueryIterator<dynamic>(query);

            var result = new List<(string, string)>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                foreach (var item in response)
                {
                    result.Add((item.categoryId.ToString(), item.categoryName.ToString()));
                }
            }

            return result;
        }
    }
}
