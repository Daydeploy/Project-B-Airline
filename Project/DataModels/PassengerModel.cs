using System.Text.Json.Serialization;

public class PassengerModel : PersonModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hasPet")]
    public bool HasPet { get; set; }

    [JsonPropertyName("petDetails")]
    public PetModel PetDetails { get; set; } 

    [JsonPropertyName("allPets")]
    public List<PetModel> AllPets { get; set; } = new List<PetModel>();

    [JsonPropertyName("seatNumber")]
    public string? SeatNumber { get; set; }

    [JsonPropertyName("hasCheckedBaggage")]
    public bool HasCheckedBaggage { get; set; }

    [JsonPropertyName("shopItems")]
    public List<ShopItemModel> ShopItems { get; set; } = new List<ShopItemModel>();

    [JsonPropertyName("specialLuggage")]
    public string SpecialLuggage { get; set; }

    public PassengerModel(string name, string? seatNumber, bool hasCheckedBaggage, bool hasPet = false, PetModel petDetails = null, string specialLuggage = "")
    {
        Name = name;
        SeatNumber = seatNumber;
        HasCheckedBaggage = hasCheckedBaggage;
        HasPet = hasPet;
        PetDetails = petDetails;
        ShopItems = new List<ShopItemModel>();
        SpecialLuggage = specialLuggage;
        AllPets = new List<PetModel>();
        if (petDetails != null)
        {
            AllPets.Add(petDetails);
        }
    }
}