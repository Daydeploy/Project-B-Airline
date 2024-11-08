public class FlightsLogic
{
    public static readonly Random random = new Random();
    //public static readonly List<AirportModel> airports = AirportAccess.LoadAllAirports();

    public static List<FlightModel> AvailableFlights = new List<FlightModel>();
    public const int FlightsCount = 5;


    public static void AppendFlights()
    {
        // Load all flights from the JSON file into the AvailableFlights list
        AvailableFlights = FlightsAccess.LoadAll();

        // Optionally save the updated list to the file if needed
        FlightsAccess.WriteAll(AvailableFlights);
    }


    public List<FlightModel> GetAllFlights()
    {
        return FlightsAccess.LoadAll();
    }

    public List<FlightModel> FilterFlightsByPriceUp(string seatClass)
    {
        return AvailableFlights
            .OrderBy(f => f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MaxValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceDown(string seatClass)
    {
        return AvailableFlights
            .OrderByDescending(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MinValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceRange(string seatClass, int minPrice, int maxPrice)
    {
        return AvailableFlights
            .Where(f =>
                f.SeatClassOptions.Any(option =>
                    option.Class == seatClass && option.Price >= minPrice && option.Price <= maxPrice))
            .ToList();
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
        return GetAllFlights().Where(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<FlightModel> GetDirectFlights()
    {
        // Assuming all flights in AvailableFlights are direct flights
        return AvailableFlights;
    }

    // public List<FlightModel> GetConnectingFlights()
    // {
    //     // For this example, we'll simulate connecting flights by combining two direct flights
    //     var connectingFlights = new List<FlightModel>();
    //     var allFlights = GetAllFlights();
    //
    //     foreach (var flight1 in allFlights)
    //     {
    //         foreach (var flight2 in allFlights.Where(f => f.Origin == flight1.Destination))
    //         {
    //             var connectingFlight = new FlightModel(
    //                 flightId: random.Next(10000, 99999),
    //                 origin: flight1.Origin,
    //                 destination: flight2.Destination,
    //                 departureTime: flight1.DepartureTime,
    //                 arrivalTime: flight2.ArrivalTime,
    //                 price: flight1.Price + flight2.Price,
    //                 availableSeats: Math.Min(flight1.AvailableSeats, flight2.AvailableSeats),
    //                 flightNumber: $"{flight1.FlightNumber}-{flight2.FlightNumber}"
    //             );
    //             connectingFlights.Add(connectingFlight);
    //         }
    //     }
    //
    //     return connectingFlights;
    // }
}