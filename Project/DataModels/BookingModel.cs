using System.Text.Json.Serialization;

public class BookingModel
{
    [JsonPropertyName("bookingId")] public int BookingId { get; set; }

    [JsonPropertyName("userId")] public int UserId { get; set; }

    [JsonPropertyName("flightId")] public int FlightId { get; set; }

    [JsonPropertyName("totalPrice")] public int TotalPrice { get; set; }

    [JsonPropertyName("bookingDate")] public DateTime BookingDate { get; set; }

    [JsonPropertyName("passengers")] public List<PassengerModel> Passengers { get; set; } = new List<PassengerModel>();

    [JsonPropertyName("pets")] public List<PetModel> Pets { get; set; } = new List<PetModel>();

    [JsonPropertyName("comfortPackages")] public List<ComfortPackageModel>? ComfortPackages { get; set; }

    [JsonPropertyName("planeType")] public string? PlaneType { get; set; }

    [JsonPropertyName("entertainment")] public List<EntertainmentModel>? Entertainment { get; set; }

    public BookingModel()
    {
        // Default constructor for deserialization
    }

    public BookingModel(int bookingId, int userId, int flightId, int totalPrice, List<PassengerModel> passengers,
        List<PetModel> pets)
    {
        BookingId = bookingId;
        UserId = userId;
        FlightId = flightId;
        TotalPrice = totalPrice;
        BookingDate = DateTime.Now;
        Passengers = passengers;
        Pets = pets;
    }
}