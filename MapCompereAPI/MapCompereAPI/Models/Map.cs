namespace MapCompereAPI.Models
{
    public class Map
    {
        public string? Creator { get; set; }
        public DateTime CreationDate { get; set; }

        //Info about map
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? SVGImage { get; set; }
        public List<CountryDTO>? Countries { get; set; }

    }
}
