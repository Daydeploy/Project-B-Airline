using System.Text.Json.Serialization;

public class ShopItemModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
