namespace MapCompereAPI.Connectors
{
    public class ScrapperConnector : IScrapperConnector
    {
        private HttpClient _client;

        public ScrapperConnector()
        {
            _client = new HttpClient();
        }

        public async Task<string> ScrapData(string keyword, string description)
        {
            var response = await _client.GetAsync("https://localhost:7106/Scrapper/CustomMap?keyword=" + keyword.Replace(" ", "_") +
                                                  "&description=" + description.Replace(" ","_"));
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
