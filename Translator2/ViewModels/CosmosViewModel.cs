using Microsoft.AspNetCore.Mvc.Rendering;
using Translator2.Models;

namespace Translator2.ViewModels
{
    public class CosmosViewModel
    {
        public List<SelectListItem> Categories { get; set; } = new();

        public List<string> SelectedCategories { get; set; } = new();

        public List<Product> Products { get; set; } = new();
    }
}
