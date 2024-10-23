public class FlightsLogic
{
    public static readonly Random random = new Random();
    public static readonly List<AirportModel> airports = AirportAccess.LoadAllAirports();

    public static List<FlightModel> AvailableFlights = new List<FlightModel>();
    public const int FlightsCount = 5;

    public static FlightModel CreateRandomFlight()
    {
        // Randomly generate a Flight ID
        int flightId = random.Next(1000, 10000);

        // Planes always take off from Rotterdam The Hague airport.
        string origin = "Rotterdam";
        AirportModel originAirport = airports.FirstOrDefault(x => x.City == origin) ??
            throw new InvalidOperationException("Origin airport not found");

        // Filter the airports to exclude the origin and only include public airports
        var potentialDestinations = airports.Where(a => a.City != originAirport.City && a.Type == "Public").ToList();

        // Ensure Dublin and London are always included
        var mandatoryDestinations = new List<string> { "Dublin", "London" };
        var randomDestinations = potentialDestinations.Where(a => !mandatoryDestinations.Contains(a.City)).ToList();

        AirportModel destinationAirport;
        if (random.Next(2) == 0 && mandatoryDestinations.Count > 0)
        {
            string mandatoryCity = mandatoryDestinations[random.Next(mandatoryDestinations.Count)];
            destinationAirport = airports.First(a => a.City == mandatoryCity);
        }
        else
        {
            destinationAirport = randomDestinations[random.Next(randomDestinations.Count)];
        }
        string destination = destinationAirport.City;

        // Generate a departure time in the future
        DateTime _departureTime = DateTime.Now.AddHours(random.Next(1, 48));
        string departureTime = _departureTime.ToString("yyyy-MM-ddTHH:mm:ss");

        DateTime _arrivalTime = _departureTime.AddHours(random.Next(1, 8));
        string arrivalTime = _arrivalTime.ToString("yyyy-MM-ddTHH:mm:ss");

        // Generate random price and seats
        int price = Math.Max(random.Next(50, 500), random.Next(50, 500));
        int availableSeats = 100;
        string flightNumber = $"FL{random.Next(1000, 9999)}";

        // Return the new FlightModel
        return new FlightModel(flightId, origin, destination, departureTime, arrivalTime, price, availableSeats,
            flightNumber);
    }

    public static void AppendFlights()
    {
        for (int i = 0; i < FlightsCount; i++)
        {
            FlightModel flight = CreateRandomFlight();
            AvailableFlights.Add(flight);
        }

        FlightsAccess.WriteAll(AvailableFlights);
    }

    public List<FlightModel> GetAllFlights()
    {
        return FlightsAccess.LoadAll();
    }

    public List<FlightModel> FilterFlightsByPriceUp()
    {
        return AvailableFlights.OrderBy(f => f.Price).ToList();
    }

    public List<FlightModel> FilterFlightsByPriceDown()
    {
        return AvailableFlights.OrderByDescending(f => f.Price).ToList();
    }

    public List<FlightModel> FilterFlightsByPriceRange(int minPrice, int maxPrice)
    {
        return AvailableFlights.Where(f => f.Price >= minPrice && f.Price <= maxPrice).ToList();
    }

    public List<FlightModel> FilterFlightsByDestination(string destination)
    {
        return AvailableFlights.Where(f => f.Destination == destination).ToList();
    }

    public List<string> GetAllDestinations()
    {
        return GetAllFlights().Select(f => f.Destination).Distinct().ToList();
    }

    public List<FlightModel> SearchFlightsByDestination(string destination)
    {
        return GetAllFlights().Where(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<FlightModel> GetDirectFlights()
    {
        // Assuming all flights in AvailableFlights are direct flights
        return AvailableFlights;
    }

    public List<FlightModel> GetConnectingFlights()
    {
        // For this example, we'll simulate connecting flights by combining two direct flights
        var connectingFlights = new List<FlightModel>();
        var allFlights = GetAllFlights();

        foreach (var flight1 in allFlights)
        {
            foreach (var flight2 in allFlights.Where(f => f.Origin == flight1.Destination))
            {
                var connectingFlight = new FlightModel(
                    flightId: random.Next(10000, 99999),
                    origin: flight1.Origin,
                    destination: flight2.Destination,
                    departureTime: flight1.DepartureTime,
                    arrivalTime: flight2.ArrivalTime,
                    price: flight1.Price + flight2.Price,
                    availableSeats: Math.Min(flight1.AvailableSeats, flight2.AvailableSeats),
                    flightNumber: $"{flight1.FlightNumber}-{flight2.FlightNumber}"
                );
                connectingFlights.Add(connectingFlight);
            }
        }

        return connectingFlights;
    }
}