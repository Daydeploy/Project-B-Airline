using System.Text.Json.Serialization;

public class BookingModel
{
    [JsonPropertyName("bookingId")]
    public int BookingId { get; set; }

    // [JsonPropertyName("passengers")]
    // public List<PassengerModel> Passengers { get; set; }

    [JsonPropertyName("flighId")]
    public int FlightId { get; set; }

    [JsonPropertyName("totalPrice")]
    public int TotalPrice { get; set; }

    public BookingModel(int bookingId, FlightModel flight, int totalPrice)
    {
        BookingId = bookingId;
        FlightId = flight.FlightId;
        TotalPrice = totalPrice;
    }
}