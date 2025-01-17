public class MilesLogic
{
    private const int _silverMin = 101;
    private const int _goldMin = 201;
    private const int _platinumMin = 301;

    private const int _short_Max = 500;
    private const int _medium_Max = 2500;

    private static readonly Dictionary<(string flightType, string seatClass), int> ExperiencePoints = new()
    {
        { ("Short", "Economy"), 3 },
        { ("Short", "Business"), 6 },
        { ("Short", "First"), 10 },

        { ("Medium", "Economy"), 7 },
        { ("Medium", "Business"), 14 },
        { ("Medium", "First"), 20 },

        { ("Long", "Economy"), 10 },
        { ("Long", "Business"), 20 },
        { ("Long", "First"), 30 }
    };

    public static string CalculateLevel(int experience)
    {
        if (experience >= _platinumMin)
            return Levels.Platinum.ToString();
        if (experience >= _goldMin)
            return Levels.Gold.ToString();
        if (experience >= _silverMin)
            return Levels.Silver.ToString();
        return Levels.Bronze.ToString();
    }

    public static void UpdateAllAccountLevels()
    {
        var _accounts = AccountsAccess.LoadAll();


        foreach (var account in _accounts)
        foreach (var miles in account.Miles)
            if (miles.Enrolled)
            {
                var newLevel = CalculateLevel(miles.Experience);
                if (miles.Level != newLevel)
                {
                    miles.Level = newLevel;
                    miles.History += $"\nLevel updated to {newLevel} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                }
            }

        AccountsAccess.WriteAll(_accounts);
    }

    public static string DetermineFlightType(int distance)
    {
        if (distance <= _short_Max) return "Short";
        if (distance <= _medium_Max) return "Medium";
        return "Long";
    }

    public static string DetermineSeatClass(FlightModel flight, PassengerModel passenger)
    {
        return flight.SeatClassOptions.FirstOrDefault()?.SeatClass ?? "Economy";
    }

    public static int CalculateExperiencePoints(FlightModel flight, string seatClass)
    {
        if (!DateTime.TryParse(flight.DepartureTime, out var departureTime) ||
            departureTime > DateTime.Now)
            return 0;

        var flightType = DetermineFlightType(flight.Distance);

        if (ExperiencePoints.TryGetValue((flightType, seatClass), out var xp)) return xp;

        return 0;
    }

    public static bool UpdateFlightExperience(int accountId)
    {
        var accounts = AccountsAccess.LoadAll();
        var bookings = BookingAccess.LoadAll();
        var flights = FlightsAccess.LoadAll();

        var account = accounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null || account.Miles == null || account.Miles.Count == 0) return false;

        var milesRecord = account.Miles[0];

        if (!milesRecord.Enrolled) return true;

        var userBookings = bookings.Where(b => b.UserId == accountId).ToList();

        foreach (var booking in userBookings)
        {
            var flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
            if (flight == null) continue;

            foreach (var passenger in booking.Passengers)
            {
                var seatClass = DetermineSeatClass(flight, passenger);

                var xp = CalculateExperiencePoints(flight, seatClass);

                if (xp > 0)
                {
                    milesRecord.Experience += xp;
                    milesRecord.History +=
                        $"\nEarned {xp} XP from flight {flight.FlightNumber} ({flight.Origin} to {flight.Destination}) - {seatClass} class at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                }
            }
        }

        milesRecord.Level = CalculateLevel(milesRecord.Experience);

        AccountsAccess.WriteAll(accounts);
        return true;
    }

    public static (int earnedMiles, bool success) CalculateMilesFromBooking(int accountId)
    {
        var accounts = AccountsAccess.LoadAll();
        var bookings = BookingAccess.LoadAll();

        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null || account.Miles == null || account.Miles.Count == 0) return (0, false);

        var milesRecord = account.Miles[0];

        if (!milesRecord.Enrolled) return (0, true);

        var currentLevel = milesRecord.Level;

        var userBookings = bookings.Where(b => b.UserId == accountId).ToList();

        var totalMilesEarned = 0;

        foreach (var booking in userBookings)
        {
            var milesMultiplier = currentLevel switch
            {
                "Bronze" => 4,
                "Silver" => 6,
                "Gold" => 7,
                "Platinum" => 8,
                _ => 4
            };

            var bookingMiles = booking.TotalPrice * milesMultiplier;
            totalMilesEarned += bookingMiles;

            milesRecord.History +=
                $"\nEarned {bookingMiles} Miles from booking {booking.BookingId} - {booking.TotalPrice} euros at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        milesRecord.Points += totalMilesEarned;
        AccountsAccess.WriteAll(accounts);
        return (totalMilesEarned, true);
    }

    public static (int finalPrice, bool success) BasicPointsRedemption(int accountId, int price, int bookingId)
    {
        var accounts = AccountsAccess.LoadAll();
        var bookings = BookingAccess.LoadAll();

        var account = accounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null || account.Miles == null || account.Miles.Count == 0) return (price, false);

        var milesRecord = account.Miles[0];
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

        if (booking == null) return (price, false);

        if (milesRecord.Points >= 50000)
        {
            var discountPercentage = milesRecord.Level switch
            {
                "Bronze" => 0.05,
                "Silver" => 0.10,
                "Gold" => 0.15,
                "Platinum" => 0.20,
                _ => 0.05
            };

            var discountAmount = (int)(price * discountPercentage);
            milesRecord.Points -= 50000;
            milesRecord.History +=
                $"\nRedeemed 50000 points for {discountAmount} euro discount at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            booking.TotalPrice -= discountAmount;

            AccountsAccess.WriteAll(accounts);
            BookingAccess.WriteAll(bookings);

            return (price - discountAmount, true);
        }

        return (price, true);
    }

    private enum Levels
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }
}