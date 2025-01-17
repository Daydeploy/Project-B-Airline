using System.Text.Json.Serialization;

public class FlightModel
{
    public FlightModel(
        int flightId, string flightNumber, string origin, string originCode, string destination,
        string destinationCode, string departureTime, string arrivalTime, int distance,
        string planeType, string departureTerminal, string arrivalTerminal, string departureGate,
        string arrivalGate, List<SeatClassOption> seatClassOptions, string status, List<string> mealService,
        Taxes taxes)
    {
        FlightId = flightId;
        FlightNumber = flightNumber;
        Origin = origin;
        OriginCode = originCode;
        Destination = destination;
        DestinationCode = destinationCode;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Distance = distance;
        PlaneType = planeType;
        DepartureTerminal = departureTerminal;
        ArrivalTerminal = arrivalTerminal;
        DepartureGate = departureGate;
        ArrivalGate = arrivalGate;
        SeatClassOptions = seatClassOptions;
        Status = status;
        MealService = mealService;
        Taxes = taxes;
    }

    [JsonPropertyName("id")] public int FlightId { get; set; }

    [JsonPropertyName("flightNumber")] public string FlightNumber { get; set; }

    [JsonPropertyName("origin")] public string Origin { get; set; }

    [JsonPropertyName("originCode")] public string OriginCode { get; set; }

    [JsonPropertyName("destination")] public string Destination { get; set; }

    [JsonPropertyName("destinationCode")] public string DestinationCode { get; set; }

    [JsonPropertyName("departureTime")] public string DepartureTime { get; set; }

    [JsonPropertyName("arrivalTime")] public string ArrivalTime { get; set; }

    [JsonPropertyName("distance")] public int Distance { get; set; }

    [JsonPropertyName("planeType")] public string PlaneType { get; set; }

    [JsonPropertyName("departureTerminal")]
    public string DepartureTerminal { get; set; }

    [JsonPropertyName("arrivalTerminal")] public string ArrivalTerminal { get; set; }

    [JsonPropertyName("departureGate")] public string DepartureGate { get; set; }

    [JsonPropertyName("arrivalGate")] public string ArrivalGate { get; set; }

    [JsonPropertyName("seatClassOptions")] public List<SeatClassOption> SeatClassOptions { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; }

    [JsonPropertyName("mealService")] public List<string> MealService { get; set; }

    [JsonPropertyName("taxes")] public Taxes Taxes { get; set; }
}