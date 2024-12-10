namespace ScrapperService.Helpers
{
    public class UrlCreator
    {
        public static string CreateUNSDUrl(string keyword)
        {
            string url = "https://data.un.org/Search.aspx?q=" + keyword;
            return url;
        }
    }
}
