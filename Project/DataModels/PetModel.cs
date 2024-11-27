using System.Text.Json.Serialization;

public class PetModel
{
    [JsonPropertyName("petId")]
    public int PetId { get; set; }

    [JsonPropertyName("bookingId")]
    public int BookingId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("size")]
    public string Size { get; set; }

    [JsonPropertyName("weight")]
    public double Weight { get; set; }

    [JsonPropertyName("storageLocation")]
    public string StorageLocation { get; set; }  // "Cargo" or "Storage"

    [JsonPropertyName("maxWeight")]
    public double MaxWeight { get; set; }

    [JsonPropertyName("seatingLocation")]
    public string SeatingLocation { get; set; }

    [JsonPropertyName("fee")]
    public Dictionary<string, decimal> Fee { get; set; }
}