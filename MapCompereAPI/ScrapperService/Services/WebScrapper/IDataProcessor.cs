
namespace ScrapperService.Services.WebScrapper
{
    public interface IDataProcessor
    {
        Task<string> ProcessMdData(string mdString, string query);
    }
}