using System.Text.Json.Serialization;

public class FlightModel
{
    [JsonPropertyName("id")]
    public int FlightId { get; set; }

    [JsonPropertyName("origin")]
    public string Origin { get; set; }

    [JsonPropertyName("destination")]
    public string Destination { get; set; }

    [JsonPropertyName("departureTime")]
    public string DepartureTime { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("availableSeats")]
    public int AvailableSeats { get; set; }

    public FlightModel(int flightId, string origin, string destination, string departureTime, int price, int availableSeats)
    {
        FlightId = flightId;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        Price = price;
        AvailableSeats = availableSeats;
    }
}