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

    // Retrieve all flights from the data source
    public List<FlightModel> GetAllFlights()
    {
        return FlightsAccess.LoadAll();
    }

    // Retrieve a specific flight by its ID
    public FlightModel GetFlightsById(int flightId)
    {
        return AvailableFlights.FirstOrDefault(f => f.FlightId == flightId);
    }

    // Filter flights by price (low to high) and origin
    public List<FlightModel> FilterFlightsByPriceUp(string origin, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MaxValue)
            .ToList();
    }

    // Filter flights by price (high to low) and origin
    public List<FlightModel> FilterFlightsByPriceDown(string origin, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(f => f.SeatClassOptions.FirstOrDefault(option => option.Class == seatClass)?.Price ?? int.MinValue)
            .ToList();
    }

    // Filter flights within a specific price range and origin
    public List<FlightModel> FilterFlightsByPriceRange(string origin, string seatClass, int minPrice, int maxPrice)
    {
        return AvailableFlights
            .Where(f =>
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                f.SeatClassOptions.Any(option =>
                    option.Class == seatClass && option.Price >= minPrice && option.Price <= maxPrice))
            .ToList();
    }

    // Filter flights by destination and origin
    public List<FlightModel> FilterFlightsByDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f => 
                f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) && 
                f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Get a list of unique destinations from available flights, optionally filtered by origin
    public List<string> GetDestinationsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase))
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    // Get a list of unique destinations from all flights
    public List<string> GetAllDestinations()
    {
        return AvailableFlights.Select(f => f.Destination).Distinct().ToList();
    }

    // Get a list of unique origins from available flights
    public List<string> GetAllOrigins()
    {
        return AvailableFlights.Select(f => f.Origin).Distinct().ToList();
    }

    // Filter flights by a specific date range and origin
    public List<FlightModel> FilterByDateRange(string origin, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight =>
                flight.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(flight.DepartureTime) >= startDate &&
                DateTime.Parse(flight.DepartureTime) <= endDate)
            .ToList();
    }

    // Retrieve flights based on origin
    public List<FlightModel> GetFlightsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // Advanced filtering for flights based on multiple criteria
    public List<FlightModel> FilterFlights(string destination, DateTime startDate, DateTime endDate, string origin)
    {
        return AvailableFlights
            .Where(flight =>
                flight.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                flight.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase) &&
                DateTime.Parse(flight.DepartureTime) >= startDate &&
                DateTime.Parse(flight.DepartureTime) <= endDate)
            .ToList();
    }
}
