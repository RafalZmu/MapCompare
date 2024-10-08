namespace ScrapperService.Services
{
    public interface IScrapperService
    {
        Task<List<string>> GetDatasetsTitles(string url, HttpClient http);

        Task<string> DownloadData(string url, int dataIndexToDownload);

        List<Dictionary<string, string>> ProcessData(int FileIndex);
        
    }
}