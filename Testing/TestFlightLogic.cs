[TestClass]
public class TestFlightLogic
{
    private FlightsLogic _flightsLogic;
    private List<FlightModel> _testFlights;

    [TestInitialize]
    public void Setup()
    {
        _flightsLogic = new FlightsLogic();
        
        // Create test flight data
        _testFlights = new List<FlightModel>
        {
            new FlightModel(
                1, "Rotterdam", "Paris", "2024-11-15 10:00", "2024-11-15 12:00", null, 0, 0, null, null, null, null, null, 
                new List<SeatClassOption>
                {
                    new SeatClassOption("Economy", 150),
                    new SeatClassOption("Business", 300)
                }, null, null),
            new FlightModel(
                2, "Rotterdam", "London", "2024-11-16 14:00", "2024-11-16 15:30", null, 0, 0, null, null, null, null, null, 
                new List<SeatClassOption>
                {
                    new SeatClassOption("Economy", 200),
                    new SeatClassOption("Business", 400)
                }, null, null),
            new FlightModel(
                3, "Rotterdam", "London", "2024-11-17 09:00", "2024-11-17 10:30", null, 0, 0, null, null, null, null, null, 
                new List<SeatClassOption>
                {
                    new SeatClassOption("Economy", 175),
                    new SeatClassOption("Business", 350)
                }, null, null)
        };

        // Set the test flights in the AvailableFlights list
        FlightsLogic.AvailableFlights = _testFlights;
    }

    [TestMethod]
    public void TestFilterFlightsByDestination()
    {
        // Act
        var londonFlights = _flightsLogic.FilterFlightsByDestination("London");
        var parisFlights = _flightsLogic.FilterFlightsByDestination("Paris");
        var berlinFlights = _flightsLogic.FilterFlightsByDestination("Berlin");

        // Assert
        Assert.AreEqual(2, londonFlights.Count, "Should find 2 flights to London");
        Assert.AreEqual(1, parisFlights.Count, "Should find 1 flight to Paris");
        Assert.AreEqual(0, berlinFlights.Count, "Should find no flights to Berlin");
        
        Assert.IsTrue(londonFlights.All(f => f.Destination == "London"), "All flights should be to London");
        Assert.IsTrue(parisFlights.All(f => f.Destination == "Paris"), "All flights should be to Paris");
    }

    [TestMethod]
    public void TestFilterByDateRange()
    {
        // Arrange
        var startDate = DateTime.Parse("2024-11-15");
        var endDate = DateTime.Parse("2024-11-17");
        
        // Act
        var filteredFlights = _flightsLogic.FilterByDateRange(startDate, endDate);

        // Assert
        Assert.AreEqual(2, filteredFlights.Count, "Should find 2 flights within the date range");
        Assert.IsTrue(filteredFlights.All(f => 
            DateTime.Parse(f.DepartureTime) >= startDate && 
            DateTime.Parse(f.DepartureTime) <= endDate
        ), "All flights should be within the specified date range");
    }

    [TestMethod]
    public void TestFilterFlightsByOriginAndDestination()
    {
        // Arrange
        var startDate = DateTime.Parse("2024-11-15");
        var endDate = DateTime.Parse("2024-11-17");
        
        // Act
        var rotterdamToLondonFlights = _flightsLogic.FilterFlights(
            "London", startDate, endDate, "Rotterdam", "London");

        // Debug
        Console.WriteLine($"Rotterdam to London Flights Count: {rotterdamToLondonFlights.Count}");
        foreach (var flight in rotterdamToLondonFlights)
        {
            Console.WriteLine($"Flight ID: {flight.FlightId}, Origin: {flight.Origin}, Destination: {flight.Destination}, Departure: {flight.DepartureTime}");
        }

        // Assert
        Assert.AreEqual(2, rotterdamToLondonFlights.Count, "Should find 2 flight from Rotterdam to London");
                
        Assert.IsTrue(rotterdamToLondonFlights.All(f => 
            f.Origin == "Rotterdam" && f.Destination == "London"
        ), "Flight should be from Rotterdam to London");
        
    }

    [TestMethod]
    public void TestCombinedFiltering()
    {
        // Arrange
        var startDate = DateTime.Parse("2024-11-15");
        var endDate = DateTime.Parse("2024-11-17");
        
        // Act
        var filteredFlights = _flightsLogic.FilterFlights(
            "London", startDate, endDate, "Rotterdam", "London");

        // Assert
        Assert.AreEqual(1, filteredFlights.Count, "Should find 1 flight matching all criteria");
        
        var flight = filteredFlights.First();
        Assert.AreEqual("Rotterdam", flight.Origin);
        Assert.AreEqual("London", flight.Destination);
        Assert.IsTrue(DateTime.Parse(flight.DepartureTime) >= startDate);
        Assert.IsTrue(DateTime.Parse(flight.DepartureTime) <= endDate);
    }

    [TestMethod]
    public void TestGetAllDestinations()
    {
        // Act
        var destinations = _flightsLogic.GetAllDestinations();

        // Assert
        Assert.AreEqual(2, destinations.Count, "Should find 2 unique destinations");
        CollectionAssert.Contains(destinations, "London");
        CollectionAssert.Contains(destinations, "Paris");
    }
}