namespace ScrapperService.Services
{
    public interface IScrapperService
    {
        Task<List<string>> GetDatasetsTitles(string url, HttpClient http);

        Task<string> DownloadData(string url, int dataIndexToDownload);

        Task<Dictionary<string, Object>> ProcessData(int FileIndex);
        
    }
}