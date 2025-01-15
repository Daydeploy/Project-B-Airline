using System.Text.Json.Serialization;

public class SmallItemsModel
{
    // Parameterless constructor for JSON deserialization
    public SmallItemsModel()
    {
    }

    public SmallItemsModel(string category, List<ItemDetailModel> items)
    {
        Category = category;
        Items = items;
    }

    [JsonPropertyName("category")] public string Category { get; set; }

    [JsonPropertyName("items")] public List<ItemDetailModel> Items { get; set; }
}