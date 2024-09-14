
namespace ScrapperService.Connectors
{
    public interface ILLMServiceConnector
    {
        Task<string> GetPrediction(string query, string instructions = "");
    }
}