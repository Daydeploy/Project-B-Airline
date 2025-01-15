using System.Text.Json.Serialization;

public class SmallItemsModel
{
    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("items")]
    public List<ItemDetailModel> Items { get; set; }

    // Parameterless constructor for JSON deserialization
    public SmallItemsModel() { }
    
    public SmallItemsModel(string category, List<ItemDetailModel> items)
    {
        Category = category;
        Items = items;
    }
}
