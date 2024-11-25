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

    [JsonPropertyName("seatingLocation")]
    public string SeatingLocation { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }
} 