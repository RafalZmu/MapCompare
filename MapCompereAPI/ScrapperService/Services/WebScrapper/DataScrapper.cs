﻿using HtmlAgilityPack;
using ScrapperService.Helpers;
using System.Reflection.Metadata.Ecma335;

namespace ScrapperService.Services.WebScrapper
{
    public class DataScrapper
    {
        private readonly IDataProcessor _dataProcessor;
        public DataScrapper(IDataProcessor dataProcessor)
        {
            _dataProcessor = dataProcessor; 
        }
        public async Task<string> ScrapData(string keyword, string description)
        {
            var url = UrlCreator.CreateBraveUrl("Countries "+keyword +" data");
            var websiteLink = ScrapeWebsiteLink(url);

            var mdSourceWebsite = GetMdWebsite(websiteLink);

            var data = await _dataProcessor.ProcessMdData(mdSourceWebsite, keyword+" "+description);
            return websiteLink;
        }


        public static string ScrapeWebsiteLink(string url)
        {
            var httpClient = new HttpClient();
            var text = httpClient.GetStringAsync(url).Result;
            var html = new HtmlDocument();
            html.LoadHtml(text);
            var link = html.DocumentNode.SelectSingleNode("//a[contains(@class, 'svelte-yo6adg') and contains(@class, 'heading-serpresult')]");
            var href = link.GetAttributeValue("href", string.Empty);

            return href;
        }
        private string GetMdWebsite(string websiteLink)
        {
            var httpClient = new HttpClient();
            var md = httpClient.GetStringAsync("https://r.jina.ai/"+websiteLink).Result;
            return md;
        }
    }
}
