﻿
namespace MapCompereAPI.Connectors
{
    public interface IScrapperConnector
    {
        Task<string> ScrapData(string keyword, string description);
        Task<string> ScrapDataFromWeb(string keyword, string description);
    }
}