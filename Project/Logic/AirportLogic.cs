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

    public void UpdateAirport(AirportModel updatedAirport)
    {
        var existingAirport = _airports.FirstOrDefault(a => a.AirportID == updatedAirport.AirportID);
        if (existingAirport == null)
        {
            throw new KeyNotFoundException($"Airport with ID {updatedAirport.AirportID} not found");
        }

        int index = _airports.IndexOf(existingAirport);
        _airports[index] = updatedAirport;

        AirportAccess.WriteAllAirports(_airports);
        _airports = AirportAccess.LoadAllAirports();
    }
}