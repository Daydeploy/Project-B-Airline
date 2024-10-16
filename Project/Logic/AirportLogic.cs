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

    public List<AirportModel> GetLuxuriousAirports()
    {
        return _airports.Where(a => a.IsLuxurious).ToList();
    }

    public AirportModel GetAirportById(int id)
    {
        return _airports.FirstOrDefault(a => a.AirportID == id);
    }
}