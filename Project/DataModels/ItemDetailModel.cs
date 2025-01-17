using System.Text.Json.Serialization;

public class ItemDetailModel
{
    // Parameterless constructor for JSON deserialization
    public ItemDetailModel()
    {
    }

    // Constructor for creating new instances
    public ItemDetailModel(string name, double price, string description)
    {
        Name = name;
        Price = price;
        Description = description;
    }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("price")] public double Price { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }
}