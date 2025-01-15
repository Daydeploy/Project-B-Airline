using System.Text.Json.Serialization;

public class SeatClassOption
{
    // Parameterless constructor for JSON deserialization
    public SeatClassOption()
    {
    }

    public SeatClassOption(string seatClass, int price)
    {
        SeatClass = seatClass;
        Price = price;
    }

    [JsonPropertyName("class")] public string SeatClass { get; set; }

    [JsonPropertyName("price")] public int Price { get; set; }

    [JsonPropertyName("seasonalMultiplier")]
    public SeasonalMultiplier? SeasonalMultiplier { get; set; }
}