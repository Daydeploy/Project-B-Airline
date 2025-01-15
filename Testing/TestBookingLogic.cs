using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestBookingLogic
{
    private static readonly List<PassengerModel> _testPassengers = new List<PassengerModel>
    {
        new PassengerModel("Klaas Dijkman", "1A", true, false, new List<PetModel>(), "")
        {
            NumberOfBaggage = 1
        }
    };

    private static readonly List<PetModel> _testPets = new List<PetModel>
    {
        new PetModel
        {
            Type = "Cat",
            Weight = 5,
            SeatingLocation = "Cabin"
        }
    };

    [TestMethod]
    public void TestCreateBooking_ValidRegularFlight_ReturnsBooking()
    {
        int userId = 1;
        int flightId = 1384; // echt flight ID

        var booking = BookingLogic.CreateBooking(userId, flightId, _testPassengers, new List<PetModel>());

        Assert.IsNotNull(booking);
        Assert.AreEqual(userId, booking.UserId);
        Assert.AreEqual(flightId, booking.FlightId);
        Assert.AreEqual(1, booking.Passengers.Count);
        Assert.IsFalse(string.IsNullOrEmpty(booking.Passengers[0].SeatNumber));
    }

    [TestMethod]
    public void TestCreateBooking_ValidPrivateJet_ReturnsBooking()
    {
        int userId = 1;
        string jetType = "Bombardier Learjet 75";

        var booking = BookingLogic.CreateBooking(userId, 0, _testPassengers, new List<PetModel>(), true, jetType);

        Assert.IsNotNull(booking);
        Assert.AreEqual(userId, booking.UserId);
        Assert.AreEqual(jetType, booking.PlaneType);
        Assert.AreEqual(15000, booking.TotalPrice);
    }

    [TestMethod]
    public void TestCreateBooking_InvalidFlightId_ReturnsNull()
    {
        int userId = 1;
        int invalidFlightId = -1;

        var booking = BookingLogic.CreateBooking(userId, invalidFlightId, _testPassengers, new List<PetModel>());

        Assert.IsNull(booking);
    }

    [TestMethod]
    public void TestCreateBooking_WithPets_IncludesPetFees()
    {
        int userId = 1;
        int flightId = 1384;

        var booking = BookingLogic.CreateBooking(userId, flightId, _testPassengers, _testPets);

        Assert.IsNotNull(booking);
        Assert.IsTrue(booking.TotalPrice > 0);
        Assert.AreEqual(1, booking.Pets.Count);
    }

    [TestMethod]
    public void TestGetBookingsForUser_ReturnsCorrectBookings()
    {
        int userId = 1;

        var bookings = BookingLogic.GetBookingsForUser(userId);

        Assert.IsNotNull(bookings);
        Assert.IsTrue(bookings.All(b => b.UserId == userId));
    }

    [TestMethod]
    public void TestGetBookingsForFlight_ReturnsCorrectBookings()
    {
        int flightId = 1384;

        var bookings = BookingLogic.GetBookingsForFlight(flightId);

        Assert.IsNotNull(bookings);
        Assert.IsTrue(bookings.All(b => b.FlightId == flightId));
    }

    [TestMethod]
    public void TestTryCheckIn_ValidBooking_ReturnsSuccess()
    {
        var booking = BookingLogic.CreateBooking(1, 1384, _testPassengers, new List<PetModel>());
        Assert.IsNotNull(booking);

        var (success, message) = BookingLogic.TryCheckIn(booking.BookingId);

        Assert.IsTrue(success);
        Assert.AreEqual("Successfully checked in.", message);
    }

    [TestMethod]
    public void TestTryCheckIn_InvalidBooking_ReturnsFalse()
    {
        int invalidBookingId = -1;

        var (success, message) = BookingLogic.TryCheckIn(invalidBookingId);

        Assert.IsFalse(success);
        Assert.AreEqual("Booking not found.", message);
    }

    [TestMethod]
    public void TestGetAvailableCheckInBookings_ReturnsOnlyValidBookings()
    {
        int userId = 1;

        var bookings = BookingLogic.GetAvailableCheckInBookings(userId);

        Assert.IsNotNull(bookings);
        Assert.IsTrue(bookings.All(b => !b.IsCheckedIn));
        Assert.IsTrue(bookings.All(b => b.UserId == userId));
    }
}