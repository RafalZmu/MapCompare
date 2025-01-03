namespace ScrapperService.Connectors
{
    public class LLMServiceConnector : ILLMServiceConnector
    {
        private HttpClient _client;
        private string _serviceUrl = "http://127.0.0.1:5000";

        public LLMServiceConnector()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetPrediction(string query, string instructions = "")
        {
            string responseBody = "";
            string url = $"{_serviceUrl}/generate?query={Uri.EscapeDataString(query)}&instructions={Uri.EscapeDataString(instructions)}";
            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
            return responseBody;
        }
        public async Task<string> ExtractDataFromMd(string md, string query, string instrunctions = "")
        {
            string responseBody = "";
            string url = $"{_serviceUrl}/ExtractFromMd?md={Uri.EscapeDataString(md)}&query={Uri.EscapeDataString(query+" "+instrunctions)}";
            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
            return responseBody;

        }
    }
}
