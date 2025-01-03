
namespace ScrapperService.Connectors
{
    public interface ILLMServiceConnector
    {
        Task<string> GetPrediction(string query, string instructions = "");
        Task<string> ExtractDataFromMd(string md, string query, string instrunctions = "");
    }
}