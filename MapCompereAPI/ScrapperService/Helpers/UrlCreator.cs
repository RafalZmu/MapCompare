namespace ScrapperService.Helpers
{
    public class UrlCreator
    {
        public static string CreateUNSDUrl(string keyword)
        {
            string url = "https://data.un.org/Search.aspx?q=" + keyword.Replace(" ", "+");
            return url;
        }
        public static string CreateBraveUrl(string keyword)
        {
            string url = "https://search.brave.com/search?q=" + keyword.Replace(" ", "+"); 
            return url;
        }
    }
}
