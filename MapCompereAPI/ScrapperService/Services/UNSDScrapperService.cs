using ScrapperService.Models;
using System.Linq;

namespace ScrapperService.Services
{
    public class UNSDScrapperService : IScrapperService
    {
        private string _example_data_path = """C:\Users\rafal\Downloads\UNdata_Export_20240624_202226784\UNdata_Export_20240624_202226784.csv""";

        private List<List<string>> _rawData;  
        private UNSDFullDataContent _dataContent;

        // Ensure the file exists
        public string ReadData()
        {
            //Get the data from the file
            _rawData = GetDataFromFile(_example_data_path);

            //Get the data Title and Column names
            _dataContent = GetDataTitleAndColumnNames(_rawData[0], _rawData[1]);

            //Get the all the rows for first country and get the row context

            //Request API for decision whith row is the most relevant to the query

            //Get the data for all countries from the most relevant row

            return _dataContent.ToString();


        }

        private static List<List<string>> GetDataFromFile(string example_data_path)
        {
            if (!File.Exists(example_data_path))
            {
                Console.WriteLine("File not found.");
                throw new ApplicationException("File not found.");
            }

            try
            {
                using (StreamReader sr = new(example_data_path))
                {
                    List<List<string>> data = [];
                    while (!sr.EndOfStream) 
                    {
                        var line = sr.ReadLine();
                        List<string> values = line.Split(',').ToList();
                        data.Add(values);

                    }
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                throw new ApplicationException("The file could not be read:", e);
            }
        }

        private static UNSDFullDataContent GetDataTitleAndColumnNames(List<string> firstRow, List<string> secondRow)
        {
            var rowToExtractFrom = firstRow;
            var dataContent = new UNSDFullDataContent();

            if (firstRow.Count == 0 || firstRow.Contains(""))
            {
                Console.WriteLine("Incomplete data in first row.");

                rowToExtractFrom = secondRow;
            }
            foreach (var item in rowToExtractFrom)
            {
                dataContent.ColumnNames.Add(item);
            }

            return dataContent;
        }
    }
}
