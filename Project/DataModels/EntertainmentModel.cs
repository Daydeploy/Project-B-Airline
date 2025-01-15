using System.Text.Json.Serialization;

public class EntertainmentModel
{
    public EntertainmentModel(int id, string name, List<string> contents, decimal cost, List<string> availableIn)
    {
        Id = id;
        Name = name;
        Contents = contents;
        Cost = cost;
        AvailableIn = availableIn;
    }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("contents")] public List<string> Contents { get; set; }

    [JsonPropertyName("cost")] public decimal Cost { get; set; }

    [JsonPropertyName("availableIn")] public List<string> AvailableIn { get; set; }
}