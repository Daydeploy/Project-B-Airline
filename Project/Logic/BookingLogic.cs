public class BookingLogic
{
    private static readonly Random random = new Random();

    private static readonly List<FlightModel> _flights = FlightsAccess.LoadAll();
    private static readonly List<AccountModel> _accounts = AccountsAccess.LoadAll();
    private static readonly List<BookingModel> _bookings = BookingAccess.LoadAll();

    public static List<BookingModel> GetBookingsForFlight(int flightId)
    {
        return _bookings
            .Where(booking => booking.FlightId == flightId)
            .ToList();
    }

    public static List<BookingModel> GetBookingsForUser(int userId)
    {
        return _bookings
            .Where(booking => booking.UserId == userId)
            .ToList();
    }

    public static BookingModel CreateBooking(int userId, int flightId, List<PassengerModel> passengerDetails,
        List<PetModel> petDetails, bool isPrivateJet = false, string jetType = null)
    {
        int bookingId = GenerateBookingId();
        int totalPrice = 0;

        if (isPrivateJet && jetType != null)
        {
            var privateJetPrices = new Dictionary<string, int>
            {
                { "Bombardier Learjet 75", 15000 },
                { "Bombardier Global 8280", 25000 }
            };

            if (privateJetPrices.ContainsKey(jetType))
            {
                totalPrice = privateJetPrices[jetType];
            }
            else
            {
                return null;
            }
        }
        else
        {
            var flight = _flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight == null) return null;

            totalPrice = CalculateTotalPrice(flight.Destination, passengerDetails);

            foreach (var pet in petDetails)
            {
                var petFees = PetDataAccess.GetPetFees(pet.Type, pet.SeatingLocation);
                if (petFees != 0)
                {
                    totalPrice += (int)petFees;
                }
            }
        }

        List<PassengerModel> passengers = passengerDetails
            .Select(p => new PassengerModel(
                p.Name,
                p.SeatNumber,
                p.HasCheckedBaggage,
                p.HasPet,
                p.PetDetails,
                p.SpecialLuggage))
            .ToList();

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengers, petDetails);
        if (isPrivateJet)
        {
            newBooking.PlaneType = jetType;
        }

        _bookings.Add(newBooking);
        BookingAccess.WriteAll(_bookings);
        return newBooking;
    }


    private static int GenerateBookingId()
    {
        int bookingId;
        do
        {
            bookingId = random.Next(0, 9999);
        } while (_bookings.Any(i => i.BookingId.Equals(bookingId)));

        return bookingId;
    }

    public static (int userId, bool success) GetUserId(int id)
    {
        var account = _accounts.FirstOrDefault(x => x.Id.Equals(id));
        return account != null
            ? (account.Id, true)
            : (0, false);
    }

    public static (int flightId, bool success) GetFlightId(string destination)
    {
        var flight = _flights.FirstOrDefault(f =>
            f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        return flight != null
            ? (flight.FlightId, true)
            : (0, false);
    }

    private static int CalculateTotalPrice(string destination, List<PassengerModel> passengers)
    {
        var flight =
            _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        if (flight == null || flight.SeatClassOptions == null) return 0;

        const int baggagePrice = 30;

        var total = passengers.Sum(p =>
        {
            var seatClass = GetSeatClass(p.SeatNumber);

            var basePrice = flight.SeatClassOptions
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                ?.Price ?? 0;

            var totalPrice = basePrice + (p.HasCheckedBaggage ? baggagePrice : 0);
            return totalPrice;
        });

        // Ensure changes are saved to json
        BookingAccess.WriteAll(_bookings);
        return total;
    }

    private static string GetSeatClass(string seatNumber)
    {
        int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));

        if (row <= 3) return "First";
        if (row <= 8) return "Business";
        return "Economy";
    }

    public bool ModifyBooking(int flightId, int passengerId, BookingDetails newDetails)
    {
        var booking = _bookings.FirstOrDefault(b => b.FlightId == flightId);
        if (booking == null || booking.Passengers == null || passengerId >= booking.Passengers.Count)
        {
            return false;
        }

        var passenger = booking.Passengers[passengerId];
        passenger.SeatNumber = newDetails.SeatNumber;
        passenger.HasCheckedBaggage = newDetails.HasCheckedBaggage;

        booking.TotalPrice = CalculateTotalPrice(
            _flights.First(f => f.FlightId == flightId).Destination,
            booking.Passengers
        );

        BookingAccess.WriteAll(_bookings);
        return true;
    }

    public static List<BookingModel> GetAvailableCheckInBookings(int userId)
    {
        var flightsLogic = new FlightsLogic();
        return _bookings
            .Where(b => b.UserId == userId)
            .Where(b => !b.IsCheckedIn)
            .Where(b =>
            {
                var flight = flightsLogic.GetFlightsById(b.FlightId);
                return flight != null && DateTime.Parse(flight.DepartureTime) >= DateTime.Now;
            })
            .ToList();
    }

    public static bool CheckInBooking(int bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null || booking.IsCheckedIn)
        {
            return false;
        }

        booking.IsCheckedIn = true;
        BookingAccess.WriteAll(_bookings);
        return true;
    }

    public static (bool success, string message) TryCheckIn(int bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null)
        {
            return (false, "Booking not found.");
        }

        if (booking.IsCheckedIn)
        {
            return (false, "Booking is already checked in.");
        }

        var flightsLogic = new FlightsLogic();
        var flight = flightsLogic.GetFlightsById(booking.FlightId);
        if (flight == null)
        {
            return (false, "Flight not found.");
        }

        if (DateTime.Parse(flight.DepartureTime) < DateTime.Now)
        {
            return (false, "Flight has already departed.");
        }

        booking.IsCheckedIn = true;
        BookingAccess.WriteAll(_bookings);
        return (true, "Successfully checked in.");
    }
}