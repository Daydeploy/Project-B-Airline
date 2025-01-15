using System.Text.Json.Serialization;

public class MenuOptionModel
{
    [JsonPropertyName("menuItemId")] public int MenuItemID { get; set; }

    [JsonPropertyName("menuOptionDescription")]
    public string MenuOptionDescription { get; set; }

    [JsonPropertyName("action")] public string Action { get; set; }
}