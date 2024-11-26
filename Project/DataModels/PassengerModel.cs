using System.Text.Json.Serialization;

public class PassengerModel : PersonModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hasPet")]
    public bool HasPet { get; set; }

    [JsonPropertyName("petDetails")]
    public PetModel PetDetails { get; set; }

    [JsonPropertyName("seatNumber")]
    public string? SeatNumber { get; set; }

    [JsonPropertyName("hasCheckedBaggage")]
    public bool HasCheckedBaggage { get; set; }

    public PassengerModel(string name, string? seatNumber, bool hasCheckedBaggage, bool hasPet = false, PetModel petDetails = null)
    {
        Name = name;
        SeatNumber = seatNumber;
        HasCheckedBaggage = hasCheckedBaggage;
        HasPet = hasPet;
        PetDetails = petDetails;
    }
}