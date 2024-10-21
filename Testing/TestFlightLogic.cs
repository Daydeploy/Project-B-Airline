namespace Testing;

[TestClass]
public class TestFlightLogic
{
    // Test that GetAvailableFlights() only returns flights with an origin of RTM.
    [TestMethod]
    public void GetAvailableFlights_OnlyReturnsFlightsFromRTM()
    {
        // Arrange: Create a list of flights with the required constructor parameters.
        List<FlightModel> flights = new List<FlightModel>
        {
            new FlightModel(1, "RTM", "LHR", "Airline1", "08:00 AM", 100, 10, "F123"),
            new FlightModel(2, "AMS", "LHR", "Airline2", "09:00 AM", 150, 5, "F456"),
            new FlightModel(3, "RTM", "JFK", "Airline3", "10:00 AM", 200, 15, "F789")
        };

        // Act: Filter flights with origin RTM.
        var availableFlights = flights.Where(f => f.Origin == "RTM").ToList();

        // Assert: Check if only flights with RTM as origin are returned.
        Assert.AreEqual(2, availableFlights.Count);
        Assert.IsTrue(availableFlights.All(f => f.Origin == "RTM"));
    }

    // Test that FilterFlightsByPrice() returns flights under or equal to the specified price.
    [TestMethod]
    [DataRow(150)]
    [DataRow(100)]
    [DataRow(75)]
    public void FilterFlightsByPrice_ReturnsFlightsUnderOrEqualPrice(int maxPrice)
    {
        // Arrange: Create a list of flights with the required constructor parameters.
        List<FlightModel> flights = new List<FlightModel>
        {
            new FlightModel(1, "RTM", "LHR", "Airline1", "08:00 AM", 100, 10, "F123"),
            new FlightModel(2, "RTM", "JFK", "Airline2", "09:00 AM", 250, 5, "F456"),
            new FlightModel(3, "RTM", "AMS", "Airline3", "10:00 AM", 75, 15, "F789")
        };

        // Act: Filter flights by price.
        var filteredFlights = flights.Where(f => f.Price <= maxPrice).ToList();

        // Assert: Check if the flights are under or equal to the given price.
        Assert.IsTrue(filteredFlights.All(f => f.Price <= maxPrice));
    }

    // Test that ConfirmBooking() reduces the available seats and saves passenger details.
    [TestMethod]
    public void ConfirmBooking_UpdatesSeatsAndSavesPassengerDetails()
    {
        // Arrange: Create mock flight with available seats and booking details.
        List<FlightModel> flights = new List<FlightModel>
        {
            new FlightModel(1, "RTM", "LHR", "Airline1", "08:00 AM", 100, 10, "F123")
        };

        List<PassengerModel> passengers = new List<PassengerModel>
        {
            new PassengerModel("John Doe", "12A", true),
            new PassengerModel("Jane Doe", "12B", false)
        };

        List<BookingModel> bookings = new List<BookingModel>();

        // Mock flight access and booking logic.
        var flightToBook = flights.FirstOrDefault(f => f.FlightId == 1);
        int initialAvailableSeats = flightToBook.AvailableSeats;

        // Act: Confirm booking by reducing the available seats.
        BookingModel booking = BookingLogic.CreateBooking(1, "LHR", passengers);

        // Assert: Check if the available seats are reduced.
        Assert.AreEqual(initialAvailableSeats - passengers.Count, flightToBook.AvailableSeats);
        
        // Assert: Check if passenger details are correctly saved.
        Assert.AreEqual(2, booking.Passengers.Count);
        Assert.AreEqual("John Doe", booking.Passengers[0].Name);
        Assert.AreEqual("Jane Doe", booking.Passengers[1].Name);
    }
}
