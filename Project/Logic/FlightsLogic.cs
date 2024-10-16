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

        // Generate a departure time in the future
        DateTime departureTime = DateTime.Now.AddHours(random.Next(1, 48));
        
        // Generate an arrival time after the departure time
        DateTime arrivalTime = departureTime.AddHours(random.Next(1, 8));

        int price = Math.Max(random.Next(50, 500), random.Next(50, 500));
        int availableSeats = 100;
        string flightNumber = $"FL{random.Next(1000, 9999)}";

        return new FlightModel(flightId, origin, destination, departureTime, arrivalTime, price, availableSeats, flightNumber);
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

    public List<FlightModel> GetAllFlights()
    {
        return FlightsAccess.LoadAll();
    }

    public List<FlightModel> FilterFlightsByPriceUp(){
        return AvailableFlights.OrderBy(f => f.Price).ToList();
    }

    public List<FlightModel> FilterFlightsByPriceDown(){
        return AvailableFlights.OrderByDescending(f => f.Price).ToList();
    }
    
    public List<FlightModel> FilterFlightsByPriceRange(int minPrice, int maxPrice){
        return AvailableFlights.Where(f => f.Price >= minPrice && f.Price <= maxPrice).ToList();
    }
}
