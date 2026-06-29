using Translator2.Models;
using Microsoft.Azure.Cosmos;

namespace Translator2.Services
{
    public class CosmosService
    {
        private Microsoft.Azure.Cosmos.Container _container;

        public CosmosService(CosmosClient client, IConfiguration config)
        {
            var dbName = config["Cosmos:DatabaseName"];

            var db = client.GetDatabase(dbName);

            _container = db.GetContainer("TranslationHistory");
        }

        public async Task AddAsync(TranslationHistory item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task<List<TranslationHistory>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<TranslationHistory>(
                new QueryDefinition("SELECT * FROM c"));

            var result = new List<TranslationHistory>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }
    }
}
