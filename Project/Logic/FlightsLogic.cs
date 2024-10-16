public class FlightsLogic
{
    public static readonly Random random = new Random();
    public static readonly List<AirportModel> airports = AirportAccess.LoadAllAirports();

    public static List<FlightModel> AvailableFlights = new List<FlightModel>();
    public const int FlightsCount = 5;

    public static FlightModel CreateRadomFlight()
    {
        // Randomly generate a Flight ID
        int flightId = random.Next(1000, 10000);

        // Randomly select one of the available destinations and assign city to the origin
        AirportModel originAirport = airports[random.Next(airports.Count)];
        string origin = originAirport.City;


        // Randomly select destination airport (ensuring it's different from origin)
        AirportModel destinationAirport;

        do
        {
            destinationAirport = airports[random.Next(airports.Count)];
        } while (destinationAirport == originAirport);

        string destination = destinationAirport.City;

        // Generate a departure time in the past
        DateTime dateTime = DateTime.Now;
        DateTime departureTime = dateTime.AddHours(-random.Next(1, 48));
        string formattedDepartureTime = departureTime.ToString("yyyy-MM-ddTHH:mm:ss");

        // 
        int price = 500;
        int availableSeats = 100;

        return new FlightModel(flightId, origin, destination, formattedDepartureTime, price, availableSeats);

    }

    public static void AppendFlights()
    {
        for (int i = 0; i < FlightsCount; i++)
        {
            FlightModel flight = CreateRadomFlight();
            AvailableFlights.Add(flight);
        }

        FlightsAccess.WriteAll(AvailableFlights);
    }

}