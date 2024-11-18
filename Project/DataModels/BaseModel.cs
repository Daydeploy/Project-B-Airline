using System.Text.Json.Serialization;

public abstract class BaseModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}