using System.Text.Json.Serialization;

public class SeatClassOption
{
    [JsonPropertyName("class")]
    public string Class { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    // Constructor parameters now match the property names exactly
    public SeatClassOption(string @class, int price)
    {
        Class = @class;
        Price = price;
    }
}
