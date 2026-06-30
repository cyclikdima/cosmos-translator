using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Translator2.Models;
using Translator2.Services;
using Translator2.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Translator2.Controllers
{
    public class TranslatorController : Controller
    {

        private readonly TranslatorService _service;
        private readonly CosmosService _cosmos;
        private async Task FillTransliteration(TranslatorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                model.Result = null;
                model.FormattedResult = null;
                model.History = await _cosmos.GetAllAsync();
                return;
            }

            try
            {
                model.Result = await _service.Translate(
                    model.Text ?? "",
                    model.SourceLang,
                    model.TargetLang
                    );

                if (model.Result.Length <= 0)
                    model.FormattedResult = null;
                else if (model.Result.Length <= 25)
                    model.FormattedResult = $"{model.Result} - {model.Text}";
                else
                    model.FormattedResult = $"{model.Result}\n{model.Text}";

                if (!string.IsNullOrEmpty(model.Text) && model.SourceLang != "en")
                {
                    model.TextTransliteration = await _service.Transliterate(model.Text, model.SourceLang);
                }
                else
                {
                    model.TextTransliteration = null;
                }

                if (!string.IsNullOrEmpty(model.Result) && model.TargetLang != "en")
                {
                    model.ResultTransliteration = await _service.Transliterate(model.Result, model.TargetLang);
                }
                else
                {
                    model.ResultTransliteration = null;
                }

                await _cosmos.AddAsync(new TranslationHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    DateTime = DateTime.UtcNow,
                    SourceText = model.Text,
                    ResultText = model.Result,
                    SourceLanguage = model.SourceLang,
                    TargetLanguage = model.TargetLang,
                    SourceTransliteration = model.TextTransliteration,
                    ResultTransliteration = model.ResultTransliteration
                });

                model.History = await _cosmos.GetAllAsync();
            }
            catch (Exception ex)
            {
                model.Result = $"Error {ex.Message}";
            }
        }
        
        public TranslatorController(TranslatorService service, CosmosService cosmos)
        {
            _service = service;
            _cosmos = cosmos;
        }

        [HttpGet]
        
        public async Task<IActionResult> Index()
        {
            var model = new TranslatorViewModel();

            model.History = await _cosmos.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(TranslatorViewModel model)
        {
            await FillTransliteration(model);

            ModelState.Clear();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Change(TranslatorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Result))
            {
                return View("Index", model);
            }

            var tempLang = model.SourceLang;
            model.SourceLang = model.TargetLang;
            model.TargetLang = tempLang;

            model.Text = model.Result;
            await FillTransliteration(model);

            ModelState.Clear();

            return View("Index", model);
        }

    }
}
