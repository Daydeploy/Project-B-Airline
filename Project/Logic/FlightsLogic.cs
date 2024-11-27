public class FlightsLogic
{
    public static readonly Random random = new Random();
    public static List<FlightModel> AvailableFlights = new List<FlightModel>();

    // Append flights from the data source
    public static void AppendFlights()
    {
        // Load all flights from the data source
        AvailableFlights = FlightsAccess.LoadAll();

        // Optionally save the loaded flights back to the file if needed
        FlightsAccess.WriteAll(AvailableFlights);
    }

    // Retrieve all flights from the data source (excluding past flights)
    public List<FlightModel> GetAllFlights()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Retrieve a specific flight by its ID (only if it's not in the past)
    public FlightModel GetFlightsById(int flightId)
    {
        return AvailableFlights
            .FirstOrDefault(f => f.FlightId == flightId && DateTime.Parse(f.DepartureTime) >= DateTime.Now);
    }

    // Filter flights by price (low to high) with origin and destination, excluding past flights
    public List<FlightModel> FilterFlightsByPriceUp(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderBy(f => f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MaxValue)
            .ToList();
    }

    // Filter flights by price (high to low) with origin and destination, excluding past flights
    public List<FlightModel> FilterFlightsByPriceDown(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderByDescending(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MinValue)
            .ToList();
    }

    // Filter flights within a specific price range with origin and destination, excluding past flights
    public List<FlightModel> FilterFlightsByPriceRange(string origin, string destination, string seatClass,
        int minPrice, int maxPrice)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                f.SeatClassOptions.Any(option =>
                    option.Class == seatClass && option.Price >= minPrice && option.Price <= maxPrice) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Filter flights by destination and origin, excluding past flights
    public List<FlightModel> FilterFlightsByDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Get a list of unique destinations from available flights, optionally filtered by origin, excluding past flights
    public List<string> GetDestinationsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    // Get a list of unique destinations from all flights, excluding past flights
    public List<string> GetAllDestinations()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    // Get a list of unique origins from available flights, excluding past flights
    public List<string> GetAllOrigins()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Origin)
            .Distinct()
            .ToList();
    }

    // Filter flights by a specific date range with origin and destination, excluding past flights
    public List<FlightModel> FilterByDateRange(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight =>
                flight.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                flight.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(flight.DepartureTime) >= startDate &&
                DateTime.Parse(flight.DepartureTime) <= endDate &&
                DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Retrieve flights based on origin, excluding past flights
    public List<FlightModel> GetFlightsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Retrieve flights based on both origin and destination, excluding past flights
    public List<FlightModel> GetFlightsByOriginAndDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    // Advanced filtering for flights based on multiple criteria, excluding past flights
    public List<FlightModel> FilterFlights(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight =>
                flight.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                flight.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(flight.DepartureTime) >= startDate &&
                DateTime.Parse(flight.DepartureTime) <= endDate &&
                DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }
}