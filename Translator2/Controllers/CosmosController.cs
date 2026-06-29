using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos;
using Translator2.Models;
using Translator2.ViewModels;

namespace Translator2.Controllers
{
    public class CosmosController : Controller
    {
        private readonly Container _container;

        public CosmosController(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration["Cosmos:DatabaseName"] ?? "AppDatabase";

            var database = cosmosClient.GetDatabase(databaseName);

            _container = database.GetContainer("Products");
        }

        public async Task<IActionResult> Index(List<string>? selectedCategories)
        {
            var model = new CosmosViewModel();

            // 1. получаем категории
            var catQuery = new QueryDefinition(
                "SELECT DISTINCT c.categoryId, c.categoryName FROM c");

            var catIterator = _container.GetItemQueryIterator<dynamic>(catQuery);

            while (catIterator.HasMoreResults)
            {
                var response = await catIterator.ReadNextAsync();

                foreach (var item in response)
                {
                    model.Categories.Add(new SelectListItem
                    {
                        Value = item.categoryId.ToString(),
                        Text = item.categoryName.ToString(),
                        Selected = selectedCategories?.Contains(item.categoryId.ToString()) == true
                    });
                }
            }

            model.SelectedCategories = selectedCategories ?? new List<string>();

            // 2. если ничего не выбрано — берем все категории
            if (!model.SelectedCategories.Any() && model.Categories.Any())
            {
                model.SelectedCategories = model.Categories
                    .Select(x => x.Value)
                    .ToList();
            }

            // 3. загрузка товаров по нескольким категориям
            var queryText =
                $"SELECT * FROM c WHERE c.categoryId IN ({string.Join(",", model.SelectedCategories.Select((_, i) => $"@p{i}"))})";

            var query = new QueryDefinition(queryText);

            for (int i = 0; i < model.SelectedCategories.Count; i++)
            {
                query = query.WithParameter($"@p{i}", model.SelectedCategories[i]);
            }

            var iterator = _container.GetItemQueryIterator<Product>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                model.Products.AddRange(response);
            }

            return View(model);
        }
    }
}