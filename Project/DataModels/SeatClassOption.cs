using System.Text.Json.Serialization;

public class SeatClassOption
{
    [JsonPropertyName("class")] 
    public string SeatClass { get; set; }

    [JsonPropertyName("price")] 
    public double Price { get; set; }

    [JsonPropertyName("seasonalMultiplier")] 
    public SeasonalMultiplier? SeasonalMultiplier { get; set; }

    // Parameterless constructor for JSON deserialization
    public SeatClassOption() { }

    // Constructor for creating new instances
    public SeatClassOption(string seatClass, double price)
    {
        SeatClass = seatClass;
        Price = price;
    }
}