using Newtonsoft.Json;

namespace Translator2.Models
{
    public class TranslationHistory
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateTime { get; set; }

        public string SourceText { get; set; }
        public string ResultText { get; set; }


        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }

        public string? SourceTransliteration { get; set; }
        public string? ResultTransliteration { get; set; }

    }
}
