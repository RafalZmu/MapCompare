using Microsoft.Playwright;
using ScrapperService.Connectors;
using ScrapperService.Helpers;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

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
        public async Task<string> Scrape(string keyword, string query)
        {
            var url = UrlCreator.CreateUNSDUrl(keyword);
            //Get all the titles of the data from the page. return list of titles
            var http = new HttpClient();
            var titles = await _UNSDScrapper.GetDatasetsTitles(url, http);

            //Ask LLM service which title aligns best with the query
            string titlesString = "";
            int index = 0;
            foreach (string title in titles)
            {
                titlesString += index.ToString() + "-"+ title+"//";
                index++;
            }
            var MostRelevantTitleIndex = await _LLMConnector.GetPrediction(titlesString, $"Which one of provided Titles matches best to this query: {query}. Return only the number that corresponst to the most relevant query");

            //Download the data from the page
            await _UNSDScrapper.DownloadData(url, int.Parse(MostRelevantTitleIndex));

            //Process the downloaded data into dictionary
            var processedData = _UNSDScrapper.ProcessData(int.Parse(MostRelevantTitleIndex));

            //Extract the key value pairs from the processed data
            var Data = processedData[0];
            var keysString = "";
            foreach (var key in Data)
            {
                keysString += "|"+key.Key+","+key.Value+"|";
            }
            keysString = "["+ keysString + "]";

            //Ask LLMService which key aligns best with the query
            var mostRelevantKeys = await _LLMConnector.GetPrediction(keysString, $"In data part you will be provided comma separeted list of keys and corresponding values to some data dictionary. The data is associated with this query={query}. You need to return only two best matching keys. Your response should exactly match the template: [Description=The key with highest possibility of containing the description of the informations in the dataset must be string, Value=The key with highest possibility of containing the numerical values of the dataset]. Ignore the keys with country or year in them. The keys and values are listed in the squere brackets.");

            //Remove the unwanted keys from the processed data
            mostRelevantKeys = mostRelevantKeys.Replace("[", "").Replace("]", "").Trim();
            var DatasetDescriptionKey = mostRelevantKeys.Split(",")[0].Split("=")[1];
            var DatasetValuesKey = mostRelevantKeys.Split(",")[1].Split("=")[1];

            processedData = RemoveUnwantedKeys(processedData, mostRelevantKeys);

            List<string> allRecordsDescription = GetAllRecordDescriptions(processedData, DatasetDescriptionKey);

            //Ask LLMService which description aligns best with the query
            var mostRelevantDescription = await _LLMConnector.GetPrediction(string.Join(",", allRecordsDescription), $"Which one of provided descriptions matches best to this query: {query}. Return only the number that corresponst to the most relevant query. Don't return anything else. Always return some number");
            var mostRelevantDescriptionIndex = int.Parse(mostRelevantDescription) -1;

            //From proccessed data remove all records that do not contain the most relevant description
            processedData = processedData.Where(dict => dict.ContainsKey(DatasetDescriptionKey) && dict[DatasetDescriptionKey] == allRecordsDescription[mostRelevantDescriptionIndex]).ToList();

            //Change proccessedData to JSON
            var processedDataJson = JsonSerializer.Serialize(processedData, new JsonSerializerOptions { WriteIndented = true});

            return processedDataJson;
        }

        public static List<string> GetAllRecordDescriptions(List<Dictionary<string, string>> processedData, string datasetDescriptionKey)
        {
            ArgumentNullException.ThrowIfNull(datasetDescriptionKey);
            return processedData.Where(dict => dict.ContainsKey(datasetDescriptionKey)).Select(dict => dict[datasetDescriptionKey]).Distinct().ToList();

        }

        public static List<Dictionary<string, string>> RemoveUnwantedKeys(List<Dictionary<string, string>> data, string mostRelevantKeys)
        {
            foreach(var dictionary in data)
            {
                foreach(var key in dictionary.Keys.ToList())
                {
                    if (!mostRelevantKeys.Contains(key) && !key.Contains("Country") && !key.Contains("Year") && !key.Contains("Period"))
                    {
                        dictionary.Remove(key);
                    }
                }
            }
            return data;
        }
    }
}
