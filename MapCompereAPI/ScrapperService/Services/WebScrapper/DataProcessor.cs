
using Newtonsoft.Json;
using ScrapperService.Connectors;
using System.Text.Json;

namespace ScrapperService.Services.WebScrapper
{
    public class DataProcessor : IDataProcessor
    {
        private ILLMServiceConnector _iLLMServiceConnector;
        public DataProcessor(ILLMServiceConnector iLLMServiceConnector)
        {
            _iLLMServiceConnector = iLLMServiceConnector;
        }
        public async Task<string> ProcessMdData(string mdString, string query)
        {
            var countryNamesList = GetCountriesFromJson();

            var md = mdString;
            md = CutUnncesaryDataFromMd(md, countryNamesList);

            var response = await _iLLMServiceConnector.ExtractDataFromMd(md, query, "Extract the data from the md and format it into the json format. the json format should" +
                " have following keys: Country or Territory and corresponding key Value. After the json response add another json with keys: Period  can be one year or period in format yyyy-yyyy, Statistic Description. Separate the two json only with 'END' string" +
                " Focus on data with individual countries not on regions, You can provide them too but never return only regions. Instdes of key names in response input 1 for Country or Territory"+
                " 2 for Value and. Example of one element {1:Poland,2:15}");

            string formatedResponse = FormatResponse(response);

            return formatedResponse;
        }
        private static List<string> GetCountriesFromJson()
        {
            var json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\synonymsWithCountry.json");

            var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            List<string> countries = [.. keyValuePairs.Keys];
            countries = countries.Where(x => x.Length >= 4).ToList();
            return countries;
        }

        private string FormatResponse(string response)
        {
            response = response.Replace("```json", "");
            response = response.Replace("```", "");

            string descriptionJson = response.Split("END")[1];
            string dataJson = response.Split("END")[0];
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dataJson);
            Dictionary<string, string> description = new();
            try
            {
                description = JsonConvert.DeserializeObject<Dictionary<string, string>>(descriptionJson);
            }
            catch
            {
                var descriptionList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(descriptionJson);
                description = descriptionList[0];
            }

            if (string.IsNullOrEmpty(description["Period"]))
            {
                description["Period"] = "unknown";
            }
            if (string.IsNullOrEmpty(description["Statistic Description"]))
            {
                description["Statistic Description"] = "unknown";
            }

            foreach (var item in data)
            {
                item["Country or Territory"] = item["1"];
                item["Period"] = description["Period"];
                item["Statistic Description"] = description["Statistic Description"];
                item["Value"] = item["2"];
                item.Remove("1");
                item.Remove("2");
            }


            return JsonConvert.SerializeObject(data);
        }

        private static string CutUnncesaryDataFromMd(string md, List<string> countryNamesList)
        {
            List<string> countriesInMD = new();
            var mdString = md.ToLower();

            foreach (var country in countryNamesList)
            {
                if (mdString.Contains(country))
                {
                    countriesInMD.Add(country);
                }
            }
            countriesInMD = countriesInMD.Order().ToList();

            if (countriesInMD.Count > 60)
            {
                return ExtractMd(countriesInMD, ref mdString, 800);
            }
            if (countryNamesList.Count > 20)
            {
                return ExtractMd(countriesInMD, ref mdString, 1000);
            }
            return mdString;
        }

        private static string ExtractMd(List<string> countriesInMD, ref string mdString, int range)
        {
            List<int> countriesIndex = new List<int>();
            foreach (var country in countriesInMD)
            {
                countriesIndex.Add(mdString.IndexOf(country));
            }
            int start = countriesIndex.Min();
            int end = countriesIndex.Max();

            if (start - range > 0)
                start = start - range;
            else
                start = 0;

            if (end + range < mdString.Length)
                end = end + range;
            else
                end = mdString.Length -1;

            mdString = mdString.Substring(start, end - start);
            return mdString;
        }
    }
}
