
public class CountryDTO
{
    public string Name { get; set; }
    public double Value { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }

    public CountryDTO(string name, double value, string? color, string? description)
    {
        Name = name;
        Value = value;
        Color = color;
        Description = description;
    }
}