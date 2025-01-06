using System.Text.Json.Serialization;

public class PassengerModel : PersonModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hasPet")]
    public bool HasPet { get; set; }

    [JsonPropertyName("petDetails")]
    public List<PetModel> PetDetails { get; set; } = new List<PetModel>(); // Changed from single PetModel to List

    [JsonPropertyName("seatNumber")]
    public string? SeatNumber { get; set; }

    [JsonPropertyName("hasCheckedBaggage")]
    public bool HasCheckedBaggage { get; set; }

    [JsonPropertyName("shopItems")]
    public List<ShopItemModel> ShopItems { get; set; } = new List<ShopItemModel>();

    [JsonPropertyName("specialLuggage")]
    public string SpecialLuggage { get; set; }

    public PassengerModel(string name, string? seatNumber, bool hasCheckedBaggage, bool hasPet = false, List<PetModel> petDetails = null, string specialLuggage = "")
    {
        Name = name;
        SeatNumber = seatNumber;
        HasCheckedBaggage = hasCheckedBaggage;
        HasPet = hasPet;
        PetDetails = petDetails ?? new List<PetModel>();
        ShopItems = new List<ShopItemModel>();
        SpecialLuggage = specialLuggage;
    }
}