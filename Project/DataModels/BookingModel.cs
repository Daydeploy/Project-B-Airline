using System.Text.Json.Serialization;

public class BookingModel
{
    [JsonPropertyName("bookingId")]
    public int BookingId { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("flightId")]
    public int FlightId { get; set; }

    [JsonPropertyName("totalPrice")]
    public int TotalPrice { get; set; }

    [JsonPropertyName("passengers")]
    public List<PassengerModel> Passengers { get; set; }

    [JsonPropertyName("pets")]
    public List<PetModel> Pets { get; set; }

    [JsonPropertyName("comfortPackages")]
    public List<ComfortPackageModel> ComfortPackages { get; set; } = new List<ComfortPackageModel>();

    [JsonPropertyName("planeType")]
    public string PlaneType { get; set; }

    public BookingModel(int bookingId, int userId, int flightId, int totalPrice, List<PassengerModel> passengers, List<PetModel> pets, List<ComfortPackageModel> comfortPackages = null, string planeType = null)
    {
        BookingId = bookingId;
        UserId = userId;
        FlightId = flightId;
        TotalPrice = totalPrice;
        Passengers = passengers;
        Pets = pets;
        ComfortPackages = comfortPackages;
        PlaneType = planeType;
    }   
}