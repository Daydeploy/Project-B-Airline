using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestSeatUpgrade
{
    private SeatUpgradeService _seatUpgradeService;
    private UserAccountService _userAccountService;
    private BookingLogic _bookingLogic;
    private FlightsLogic _flightsLogic;
    private List<FlightModel> _testFlights;
    private AccountModel _testUser;

    [TestInitialize]
    public void Setup()
    {
        _seatUpgradeService = new SeatUpgradeService();
        _userAccountService = new UserAccountService();
        _bookingLogic = new BookingLogic();
        _flightsLogic = new FlightsLogic();

        _testUser = new AccountModel(
            id: 1,
            firstName: "Test",
            lastName: "User",
            dateOfBirth: DateTime.Now.AddYears(-25),
            emailAddress: "test@example.com",
            password: "password123",
            miles: null
        );

        _testFlights = new List<FlightModel>
        {
            new FlightModel(
                flightId: 1,
                flightNumber: "TF123",
                destination: "Paris",
                destinationCode: "CDG",
                departureTime: DateTime.Now.AddDays(7).ToString(),
                arrivalTime: DateTime.Now.AddDays(7).AddHours(2).ToString(),
                distance: 500,
                planeType: "Boeing 737",
                departureTerminal: "T1",
                arrivalTerminal: "T2",
                departureGate: "G1",
                arrivalGate: "G2",
                seatClassOptions: new List<SeatClassOption>
                {
                    new SeatClassOption("Economy", 200),
                    new SeatClassOption("Business", 500),
                    new SeatClassOption("First", 1000)
                },
                status: "On Time",
                mealService: new List<string> { "Standard", "Premium", "Luxury" }
            )
        };
    }

    [TestMethod]
    public void TestViewAvailableUpgrades()
    {
        var booking = CreateTestBooking("15A");

        var upgrades = _seatUpgradeService.ViewAvailableUpgrades(booking.FlightId);

        Assert.IsNotNull(upgrades);
        Assert.AreEqual(2, upgrades.Count);
        Assert.IsTrue(upgrades.Any(u => u.Class == "Business"));
        Assert.IsTrue(upgrades.Any(u => u.Class == "First"));
    }

    [TestMethod]
    public void TestUpgradeSeatWithMiles()
    {
        var booking = CreateTestBooking("15A");
        int initialMiles = _testUser.Miles;

        bool upgradeResult = _seatUpgradeService.UpgradeSeat(
            booking.FlightId,
            booking.Passengers[0].Name,
            "5A",
            useMiles: true
        );

        Assert.IsTrue(upgradeResult);
        Assert.IsTrue(_testUser.Miles < initialMiles);
    }

    [TestMethod]
    public void TestUpgradeSeatWithoutMiles()
    {
        var booking = CreateTestBooking("15A");

        bool upgradeResult = _seatUpgradeService.UpgradeSeat(
            booking.FlightId,
            booking.Passengers[0].Name,
            "5A",
            useMiles: false
        );

        Assert.IsTrue(upgradeResult);
    }

    [TestMethod]
    public void TestInvalidUpgradeAttempt()
    {
        var booking = CreateTestBooking("5A");

        bool upgradeResult = _seatUpgradeService.UpgradeSeat(
            booking.FlightId,
            booking.Passengers[0].Name,
            "15A",
            useMiles: false
        );

        Assert.IsFalse(upgradeResult);
    }

    [TestMethod]
    public void TestInsufficientMilesForUpgrade()
    {
        var booking = CreateTestBooking("15A");
        _testUser.Miles = 100;

        bool upgradeResult = _seatUpgradeService.UpgradeSeat(
            booking.FlightId,
            booking.Passengers[0].Name,
            "1A",
            useMiles: true
        );

        Assert.IsFalse(upgradeResult);
    }

    private BookingModel CreateTestBooking(string seatNumber)
    {
        var passengers = new List<PassengerModel>
        {
            new PassengerModel(
                name: "Test Passenger",
                seatNumber: seatNumber,
                hasCheckedBaggage: false
            )
        };

        return new BookingModel(
            bookingId: 1,
            userId: _testUser.Id,
            flightId: _testFlights[0].FlightId,
            totalPrice: 200,
            passengers: passengers
        );
    }
} 