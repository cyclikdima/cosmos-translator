using Newtonsoft.Json;
using System.Text;


namespace Translator2.Services
{
    public class TranslatorService
    {
        private readonly string _key;
        private readonly string _endpoint;
        private readonly string _region;

        private static readonly HttpClient _client = new HttpClient();

        public TranslatorService(IConfiguration config)
        {
            _key = config["Translator:Key"];
            _endpoint = config["Translator:Endpoint"];
            _region = config["Translator:Region"];
        }

        public async Task<string> Translate(string text, string from, string to)
        {
            string route = $"/translate?api-version=3.0&from={from}&to={to}";

            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using var request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(_endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Сервіс тимчасово недоступний");
            }

            dynamic json = JsonConvert.DeserializeObject(result);

            return json[0].translations[0].text.ToString();
        }
        public async Task<string> Transliterate(string text, string languageCode)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string route = $"/transliterate?api-version=3.0&language={languageCode}&fromScript=Cyrl&toScript=Latn";

            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(_endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _region);

            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Сервіс тимчасово недоступний");
            }

            dynamic json = JsonConvert.DeserializeObject(result);
            return json[0].text.ToString();
        }

    }
}
