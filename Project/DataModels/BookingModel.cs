using System.Text.Json.Serialization;

public class BookingModel
{
    [JsonPropertyName("bookingId")] public int BookingId { get; set; }

    [JsonPropertyName("userId")] public int UserId { get; set; }

    [JsonPropertyName("flightId")] public int FlightId { get; set; }

    [JsonPropertyName("totalPrice")] public int TotalPrice { get; set; }

    [JsonPropertyName("seatNumber")] public string SeatNumber { get; set; }

    [JsonPropertyName("hasCheckedBaggage")]
    public bool HasCheckedBaggage { get; set; }

    public BookingModel(int bookingId, int userId, int flightId, int totalPrice, string seatNumber,
        bool hasCheckedBaggage)
    {
        BookingId = bookingId;
        UserId = userId;
        FlightId = flightId;
        TotalPrice = totalPrice;
        SeatNumber = seatNumber;
        HasCheckedBaggage = hasCheckedBaggage;
    }
}