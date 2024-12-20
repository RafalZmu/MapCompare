using MapCompereAPI.Models;
using MongoDB.Bson.IO;

namespace MapCompereAPI.Services
{
    public class DataProcessingService
    {
        private Dictionary<string, string> _synonyms;
        private Dictionary<string, string> _correctCountryNames;
        private string _countrySynonymsFilePath;
        public DataProcessingService() 
        {
            _countrySynonymsFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\world.json";
            (_correctCountryNames, _synonyms) = GetDataFromJSON();

        }

        public (Dictionary<string, string> ,Dictionary<string, string>) GetDataFromJSON()
        {
            Dictionary<string, string> synonyms = new();
            Dictionary<string, string> correctCountries = new();

            var jsonFile = File.ReadAllText(_countrySynonymsFilePath);
            var deserializedJson = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountrySynonyms>>(jsonFile);

            foreach (var country in deserializedJson)
            {
                synonyms[country.alpha2] = country.id;
                synonyms[country.alpha3] = country.id;
                correctCountries[country.id] = country.name;
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(correctCountries);
            var savepath = AppDomain.CurrentDomain.BaseDirectory + @"Assets\synonyms.json";
            File.WriteAllText(savepath, json);

            return (correctCountries, synonyms);
        }
    }
}
