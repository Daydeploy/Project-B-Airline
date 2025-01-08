using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class AirportServiceTests
{
    private AirportService _service;
    private List<AirportModel> _testAirports;

    [TestInitialize]
    public void Setup()
    {
        _testAirports = new List<AirportModel>
        {
            new AirportModel(1, "Netherlands", "Rotterdam", "Rotterdam South Airport", "RSZ", "Public", "+31 10 123 4567", "Driemanssteeweg 107, 3011 WN Rotterdam, Netherlands"),
            new AirportModel(2, "Ireland", "Dublin", "Dublin Airport", "DUB", "Public", "+353 1 814 1111", "Dublin Airport, Co. Dublin, Ireland"),
            new AirportModel(3, "Test", "City", "VIP Executive Airport", "TEST", "Public", "123", "Test Address"),
            new AirportModel(4, "Test", "City", "Luxury Private Strip", "LUX", "Private", "123", "Test Address")
        };

        _service = new AirportService(_testAirports);
    }

    [TestMethod]
    public void GetLuxuriousAirports_ReturnsOnlyPublicLuxuriousAirports()
    {
        var result = _service.GetLuxuriousAirports();

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("VIP Executive Airport", result.First().Name);
        Assert.AreEqual("Public", result.First().Type);
    }

    [TestMethod]
    public void GetLuxuriousPrivateAirstrips_ReturnsOnlyPrivateLuxuriousAirports()
    {
        var result = _service.GetLuxuriousPrivateAirstrips();

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Luxury Private Strip", result.First().Name);
        Assert.AreEqual("Private", result.First().Type);
    }

    [TestMethod]
    [DataRow("VIP Airport", true)]
    [DataRow("Executive Airport", true)]
    [DataRow("Luxury Airport", true)]
    [DataRow("Regular Airport", false)]
    public void IsAirportLuxurious_ChecksNameCorrectly(string airportName, bool expected)
    {
        var airport = new AirportModel(1, "Test", "City", airportName, "TEST", "Public", "123", "Test Address");

        var result = _service.IsAirportLuxurious(airport);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetAirportDescription_ReturnsFormattedDescription()
    {
        var airport = _testAirports.First();

        var result = _service.GetAirportDescription(airport);

        StringAssert.Contains(result, airport.Name);
        StringAssert.Contains(result, "Experience luxury");
        StringAssert.Contains(result, "VIP lounges");
    }

    [TestMethod]
    public void GetAirportTransportationOptions_ReturnsExpectedOptions()
    {
        var airport = _testAirports.First();

        var result = _service.GetAirportTransportationOptions(airport);

        CollectionAssert.Contains(result.ToList(), "Taxi");
        CollectionAssert.Contains(result.ToList(), "Shuttle");
        CollectionAssert.Contains(result.ToList(), "Limousine");
    }

    [TestMethod]
    public void GetNearbyHotels_ReturnsExpectedHotels()
    {
        var airport = _testAirports.First();

        var result = _service.GetNearbyHotels(airport);

        CollectionAssert.Contains(result.ToList(), "Luxury Hotel");
        CollectionAssert.Contains(result.ToList(), "Executive Suite");
        CollectionAssert.Contains(result.ToList(), "VIP Resort");
    }
}