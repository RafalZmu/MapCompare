
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
            var md = mdString;
            List<string> countryNamesList = GetCountriesFromJson();
            //md = CutUnncesaryDataFromMd(md, countryNamesList);

            var response = await _iLLMServiceConnector.ExtractDataFromMd(md, query, "Extract the data from the md and format it into the json format. the json format should" +
                " have following keys: Country or Territory and corresponding key Value. After the json response add another json with keys: Period  can be one year or period in format yyyy-yyyy, Statistic Description. Separate the two json only with 'END' string" +
                " Focus on data with individual countries not on regions, You can provide them too but never return only regions. Instdes of key names in response input 1 for Country or Territory"+
                " 2 for Value and. Example of one element {1:Poland,2:15}");

            throw new NotImplementedException();
        }

        private static List<string> GetCountriesFromJson()
        {
            var json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\synonymsWithCountry.json");

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            List<string> countries = [.. keyValuePairs.Keys];
            countries = countries.Where(x => x.Length >= 4).ToList();
            return countries;
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
            int start = mdString.IndexOf(countriesInMD.First());
            int end = mdString.IndexOf(countriesInMD.Last());

            if (start - range > 0)
                start = start - range;
            else
                start = 0;

            if (end + range < mdString.Length)
                end = end + range;
            else
                end = mdString.Length;

            mdString = mdString.Substring(start, end - start);
            return mdString;
        }
    }
}
