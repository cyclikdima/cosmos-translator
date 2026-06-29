using System.ComponentModel.DataAnnotations;
using Translator2.Models;

namespace Translator2.ViewModels
{
    public class TranslatorViewModel
    {
        public string? Text { get; set; }
        public string? Result { get; set; }
        public string? TextTransliteration { get; set; }
        public string? ResultTransliteration { get; set; }

        public string? FormattedResult { get; set; }

        public string? SourceLang { get; set; } = "uk";
        public string? TargetLang { get; set; } = "en";

        public List<TranslationHistory>? History { get; set; }
    }
}
