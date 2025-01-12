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

    public bool DeleteAirport(string airportCode)
    {
        // Zoek de luchthaven die verwijderd moet worden en caps etc worden ignored
        var airportToDelete = _airports.FirstOrDefault(a => a.Code.Equals(airportCode, StringComparison.OrdinalIgnoreCase));

        // als de airport al niet bestaat returned die gewoon false
        if (airportToDelete == null)
            return false;

        // Dit verwijderd de airport uit de airport list
        _airports.Remove(airportToDelete);

        // Dit slaat de nieuwe airport list op
        AirportAccess.WriteAllAirports(_airports);

        // dit loadt alle flights uit de json
        var flights = FlightsAccess.LoadAll();

        // filteren op de luchthaven die verwijderd moet worden
        var updatedFlights = flights.Where(flight => 
            !flight.Origin.Equals(airportToDelete.Name, StringComparison.OrdinalIgnoreCase) &&
            !flight.Destination.Equals(airportToDelete.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // slaat de nieuwe flight list op dus alle flights die niet verwijderd moeten worden showed die dan
        FlightsAccess.WriteAll(updatedFlights);

        // als alles goed gegaan is returned die true
        return true;
    }
}