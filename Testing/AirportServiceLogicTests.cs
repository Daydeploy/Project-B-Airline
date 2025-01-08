using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class AirportServiceLogicTests
{
    private IAirportService _service;
    private List<AirportModel> _testAirports;

    [TestInitialize]
    public void Setup()
    {
        _testAirports = new List<AirportModel>
        {
            new AirportModel(1, "NL", "Rotterdam", "Rotterdam South Airport", "RSZ", "Public", "+31 10 123 4567", "Driemanssteeweg 107, 3011 WN Rotterdam, Netherlands"),
            new AirportModel(2, "Ireland", "Dublin", "Dublin Airport", "DUB", "Public", "+353 1 814 1111", "Dublin Airport, Co. Dublin, Ireland"),
            new AirportModel(3, "ENG", "London", "London City Airport", "LCY", "Public", "+44 20 7646 0088", "Hartmann Rd, Royal Docks, London E16 2PX, United Kingdom")
        };

        _service = new AirportServiceLogic(_testAirports);
    }

    [TestMethod]
    public void GetAvailableAirports_ReturnsOnlyDutchAirports()
    {
        var result = _service.GetAvailableAirports();

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AllItemsAreUnique(result);
        Assert.IsTrue(result.All(a => a.Country == "NL"));
    }

    [TestMethod]
    public void GetAvailableServices_ReturnsExpectedServices()
    {
        var airport = _testAirports.First();

        var result = _service.GetAvailableServices(airport);

        StringAssert.Contains(result, "Helicopter Transfer");
        StringAssert.Contains(result, "VIP Lounge");
        StringAssert.Contains(result, "Concierge Service");
    }

    [TestMethod]
    public void GetAirportDescription_ReturnsFormattedDescription()
    {
        var airport = _testAirports.First();

        var result = _service.GetAirportDescription(airport);

        StringAssert.Contains(result, airport.Name);
        StringAssert.Contains(result, "Experience luxury");
        StringAssert.Contains(result, "VIP lounges");
        StringAssert.Contains(result, "personalized concierge");
    }
}