namespace ScrapperService.Services
{
    public interface IScrapperService
    {
        Task<string> ReadData();
        Task<List<string>> GetDatasetsTitles(string url, HttpClient http);

        Task<string> DownloadData(string url, int dataIndexToDownload);
        
    }
}