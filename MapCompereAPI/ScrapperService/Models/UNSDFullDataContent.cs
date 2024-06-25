namespace ScrapperService.Models
{
    public class UNSDFullDataContent
    {
        public string ContextTitle; // Get from the website while scrapping 
        public List<string> ColumnNames;
        public string RelevantRowName; 
        public string FirstRelevantRowIndex;

        public UNSDFullDataContent()
        {
            ContextTitle = "";
            ColumnNames = new List<string>();
            RelevantRowName = "";
            FirstRelevantRowIndex = "";
        }
    }
    
}