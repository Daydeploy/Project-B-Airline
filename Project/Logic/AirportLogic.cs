using System.Collections.Generic;
using System.Linq;

public class AirportLogic
{
    private List<AirportModel> _airports;

    public AirportLogic()
    {
        _airports = AirportAccess.LoadAllAirports();
    }

    public List<AirportModel> GetAllAirports()
    {
        return _airports;
    }

    public AirportModel GetAirportById(int id)
    {
        return _airports.FirstOrDefault(a => a.AirportID == id);
    }

    public void AddAirport(AirportModel newAirport)
    {
        AirportAccess.AddAirport(newAirport);
        _airports = AirportAccess.LoadAllAirports();
    }

    public bool UpdateAirport(AirportModel updatedAirport)
    {
        var existingAirport = _airports.FirstOrDefault(a => a.AirportID == updatedAirport.AirportID);
        if (existingAirport == null)
        {
            return false;
        }

        int index = _airports.IndexOf(existingAirport);
        _airports[index] = updatedAirport;

        AirportAccess.WriteAllAirports(_airports);
        _airports = AirportAccess.LoadAllAirports();
        return true;
    }

    // public bool DeleteAirport(string airportCode)
    // {
    //     // Find airport to delete using code
    //     var airportToDelete = _airports.FirstOrDefault(a => 
    //         a.Code.Equals(airportCode, StringComparison.OrdinalIgnoreCase));
    
    //     if (airportToDelete == null)
    //         return false;
    
    //     // Remove airport from airports list
    //     _airports.Remove(airportToDelete);
    //     AirportAccess.WriteAllAirports(_airports);
    
    //     // Load all flights and bookings
    //     var flights = FlightsAccess.LoadAll();
    //     var bookings = BookingAccess.LoadAll();
    //     var bookedFlightIds = bookings.Select(b => b.FlightId).ToList();
    
    //     // Filter flights - keep only:
    //     // 1. Flights not connected to deleted airport OR
    //     // 2. Flights that have existing bookings
    //     var updatedFlights = flights.Where(flight => 
    //         (!flight.OriginCode.Equals(airportToDelete.Code, StringComparison.OrdinalIgnoreCase) &&
    //         !flight.DestinationCode.Equals(airportToDelete.Code, StringComparison.OrdinalIgnoreCase)) ||
    //         bookedFlightIds.Contains(flight.FlightId)
    //     ).ToList();
    
    //     // Save updated flights list
    //     FlightsAccess.WriteAll(updatedFlights);
    //     return true;
    // }
}