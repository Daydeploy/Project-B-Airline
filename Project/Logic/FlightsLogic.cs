public class FlightsLogic
{
    public static readonly Random random = new Random();
    public static List<FlightModel> AvailableFlights = new List<FlightModel>();

    public static void AppendFlights()
    {
        AvailableFlights = FlightsAccess.LoadAll();
        FlightsAccess.WriteAll(AvailableFlights);
    }

    public List<FlightModel> GetAllFlights()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public FlightModel? GetFlightsById(int flightId)
    {
        return AvailableFlights
            .FirstOrDefault(f => f.FlightId == flightId);
            // .FirstOrDefault(f => f.FlightId == flightId && DateTime.Parse(f.DepartureTime) >= DateTime.Now);
    }

    public List<FlightModel> FilterFlightsByPriceUp(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderBy(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.SeatClass == seatClass)?.Price ?? int.MaxValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceDown(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderByDescending(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.SeatClass == seatClass)?.Price ?? int.MinValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceRange(string origin, string destination, string seatClass,
        int minPrice, int maxPrice)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        f.SeatClassOptions.Any(option =>
                            option.SeatClass == seatClass && option.Price >= minPrice && option.Price <= maxPrice) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> GetReturnFlights(FlightModel selectedFlight)
    {
        return AvailableFlights
            .Where(f => f.Origin == selectedFlight.Destination &&
                        f.Destination == selectedFlight.Origin &&
                        DateTime.Parse(f.DepartureTime) > DateTime.Parse(selectedFlight.ArrivalTime))
            .ToList();
    }

    public List<string> GetDestinationsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllDestinations()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllOrigins()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Origin)
            .Distinct()
            .ToList();
    }

    public List<FlightModel> FilterByDateRange(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight => flight.Origin.Equals(origin) && flight.Destination.Equals(destination) &&
                             DateTime.Parse(flight.DepartureTime) >= startDate &&
                             DateTime.Parse(flight.DepartureTime) <= endDate &&
                             DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> GetFlightsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> GetFlightsByOriginAndDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlights(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight => flight.Origin.Equals(origin) && flight.Destination.Equals(destination) &&
                             DateTime.Parse(flight.DepartureTime) >= startDate &&
                             DateTime.Parse(flight.DepartureTime) <= endDate &&
                             DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByDate(string origin, string destination, DateTime date)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) &&
                        f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime).Date == date.Date)
            .ToList();
    }

    public void AddFlight(FlightModel newFlight)
    {
        // Validate flight
        ValidateFlight(newFlight);

        // Generate new ID
        int newId = AvailableFlights.Count > 0 ? AvailableFlights.Max(f => f.FlightId) + 1 : 1;
        newFlight.FlightId = newId;

        // Add to list and save
        AvailableFlights.Add(newFlight);
        FlightsAccess.WriteAll(AvailableFlights);
    }

    public void UpdateFlight(FlightModel updatedFlight)
    {
        // Validate flight
        ValidateFlight(updatedFlight);

        // Find and update flight
        var existingFlight = AvailableFlights.FirstOrDefault(f => f.FlightId == updatedFlight.FlightId);
        if (existingFlight == null)
            throw new KeyNotFoundException($"Flight with ID {updatedFlight.FlightId} not found");

        int index = AvailableFlights.IndexOf(existingFlight);
        AvailableFlights[index] = updatedFlight;

        // Save changes
        FlightsAccess.WriteAll(AvailableFlights);
    }

    public void DeleteFlight(int flightId)
    {
        var flight = AvailableFlights.FirstOrDefault(f => f.FlightId == flightId);
        if (flight == null)
            throw new KeyNotFoundException($"Flight with ID {flightId} not found");

        AvailableFlights.Remove(flight);
        FlightsAccess.WriteAll(AvailableFlights);
    }

    private void ValidateFlight(FlightModel flight)
    {
        if (string.IsNullOrWhiteSpace(flight.Origin))
            throw new ArgumentException("Origin is required");

        if (string.IsNullOrWhiteSpace(flight.Destination))
            throw new ArgumentException("Destination is required");

        if (string.IsNullOrWhiteSpace(flight.FlightNumber))
            throw new ArgumentException("Flight number is required");

        if (string.IsNullOrWhiteSpace(flight.DepartureTime))
            throw new ArgumentException("Departure time is required");

        if (string.IsNullOrWhiteSpace(flight.ArrivalTime))
            throw new ArgumentException("Arrival time is required");

        if (DateTime.Parse(flight.DepartureTime) >= DateTime.Parse(flight.ArrivalTime))
            throw new ArgumentException("Departure time must be before arrival time");

        if (flight.SeatClassOptions == null || !flight.SeatClassOptions.Any())
            throw new ArgumentException("At least one seat class option is required");
    }
}