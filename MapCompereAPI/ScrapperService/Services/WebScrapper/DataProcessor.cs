
using System.Text.Json;

namespace ScrapperService.Services.WebScrapper
{
    public class DataProcessor
    {
        public static string ProcessMdData(string mdString)
        {
            var md = mdString;
            List<string> countryNamesList = GetCountriesFromJson();
            md = CutUnncesaryDataFromMd(md, countryNamesList);




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
                return ExtractMd(countriesInMD, ref mdString, 100);
            }
            if (countryNamesList.Count > 20)
            {
                return ExtractMd(countriesInMD, ref mdString, 500);
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
