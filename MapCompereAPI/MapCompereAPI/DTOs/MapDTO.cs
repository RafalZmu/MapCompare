
public class MapDTO
{
    //Info about map creator and creation date
    public string Creator { get; set; }
    public DateTime CreationDate { get; set; }
    
    //Info about map
    public string Name { get; set; }
    public string Description { get; set; }
    public string SVGImage { get; set; }
    public List<CountryDTO> Countries { get; set; }

    public MapDTO(string name, string description, string svgImage, List<CountryDTO> countries)
    {

        Name = name;
        Description = description;
        SVGImage = svgImage;
        Countries = countries;
    }
}