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

    [JsonPropertyName("arrivalTime")]
    public string ArrivalTime { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("availableSeats")]
    public int AvailableSeats { get; set; }

    [JsonPropertyName("flightNumber")]
    public string FlightNumber { get; set; }

    public FlightModel(int flightId, string origin, string destination, string departureTime, string arrivalTime, int price, int availableSeats, string flightNumber)
    {
        FlightId = flightId;
        Origin = origin;
        Destination = destination;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Price = price;
        AvailableSeats = availableSeats;
        FlightNumber = flightNumber;
    }
}
