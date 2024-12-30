using MapCompereAPI.Models;

namespace MapCompereAPI.Services
{
    public class DataProcessingService
    {
        public Dictionary<string, string> synonyms;
        public Dictionary<string, string> correctCountryNames;
        private string _countrySynonymsFilePath;
        private readonly string _correctCountryNamesFilePath;
        public DataProcessingService() 
        {
            _correctCountryNamesFilePath = "correctCountries";
            _countrySynonymsFilePath =  "synonymsWithCountry";
            synonyms = GetDataFromJSON(_countrySynonymsFilePath);
            correctCountryNames = GetDataFromJSON(_correctCountryNamesFilePath);

        }

        public List<CountryDTO> JsonToCountryDto(string JsonResponse)
        {
            List<CountryDTO> countries = new();
            List<Dictionary<string, string>> scraperData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(JsonResponse);
            List<string> keys = scraperData[0].Keys.ToList();
            List<string> values = scraperData[1].Values.ToList();
            string countryKey = keys[0];
            string yearKey = keys[1];
            string descriptionKey = keys[2];
            string queryValueKey = keys[3];

            foreach (var country in scraperData)
            {
                countries.Add(new CountryDTO()
                {
                    Country = country[countryKey],
                    Year = country[yearKey],
                    Description = country[descriptionKey],
                    ValueKey = queryValueKey,
                    Value = double.Parse(country[queryValueKey])
                });
            }

            return countries;
        }


        public List<CountryDTO> CorrectCountryNames(List<CountryDTO> countries)
        {
            var correctedCountries = new List<CountryDTO>();
            foreach (var country in countries)
            {
                country.Country = country.Country.Trim().ToLower();
                if (country.Country.Contains("("))
                {
                    country.Country = country.Country.Substring(0, country.Country.IndexOf("(")).Trim();
                }    
                if (synonyms.ContainsKey(country.Country))
                {
                    country.Country = synonyms[country.Country];
                    correctedCountries.Add(country);
                    continue;
                }
                else if (correctCountryNames.ContainsValue(country.Country))
                {
                    country.Country = correctCountryNames[country.Country];
                    correctedCountries.Add(country);
                    continue;
                }
            }
            return countries;
        }


        public Dictionary<string, string> GetDataFromJSON(string fileName)
        {
            Dictionary<string, string> data = new();
            string dataPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\{fileName}.json";
            var dataJson = File.ReadAllText(dataPath);
            var dataDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);
            return dataDict;
        }
    }
}
