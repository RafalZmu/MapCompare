namespace ScrapperService.Services
{
    public interface IScrapperService
    {
        Task<string> ReadData();
    }
}