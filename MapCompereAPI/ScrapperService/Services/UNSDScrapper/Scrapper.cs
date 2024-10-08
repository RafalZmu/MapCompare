using Microsoft.Playwright;
using ScrapperService.Connectors;

namespace ScrapperService.Services.UNSDScrapper
{
    public class Scrapper
    {
        private ILLMServiceConnector _LLMConnector;
        private IScrapperService _UNSDScrapper;

        public Scrapper(ILLMServiceConnector LLMConnector, IScrapperService uNSDScrapper)
        {
            _LLMConnector = LLMConnector;
            _UNSDScrapper = uNSDScrapper;
        }
        public async Task<string> Scrape(string url, string query)
        {
            //Get all the titles of the data from the page. return list of titles
            var http = new HttpClient();
            var Titles = await _UNSDScrapper.GetDatasetsTitles(url, http);

            //Ask GTP4All service which title aligns best with the query
            var MostRelevantTitleIndex = await _LLMConnector.GetPrediction(Titles.ToString(), $"Which one of provided Titles matches best to this query: {query}. Return only the number that corresponst to the most relevant query");

            //Download the data from the page
            await _UNSDScrapper.DownloadData(url, int.Parse(MostRelevantTitleIndex));

            //Process the downloaded data into dictionari
            var processedData = await _UNSDScrapper.ProcessData(int.Parse(MostRelevantTitleIndex));




            return "";
        }

    }
}
