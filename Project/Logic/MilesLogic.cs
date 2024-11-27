public class MilesLogic
{
    // XP ranges
    private const int _bronzeMin = 0;
    private const int _silverMin = 101;
    private const int _goldMin = 201;
    private const int _platinumMin = 301;

    private enum Levels
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    // Distance range
    private const int _short_Max = 500;
    private const int _medium_Max = 2500;

    private static readonly Dictionary<(string flightType, string seatClass), int> ExperiencePoints = new()
    {
        {("Short", "Economy"), 3},
        {("Short", "Business"), 6},
        {("Short", "First"), 10},

        {("Medium", "Economy"), 7},
        {("Medium", "Business"), 14},
        {("Medium", "First"), 20},

        {("Long", "Economy"), 10},
        {("Long", "Business"), 20},
        {("Long", "First"), 30},
    };

    public static string CalculateLevel(int experience) // Returns the Level of the user as a string.
    {
        if (experience >= _platinumMin)
        {
            return Levels.Platinum.ToString();
        }
        else if (experience >= _goldMin)
        {
            return Levels.Gold.ToString();
        }
        else if (experience >= _silverMin)
        {
            return Levels.Silver.ToString();
        }
        else
        {
            return Levels.Bronze.ToString();
        }
    }

    public static void UpdateAccountLevel(int id)
    {
        List<AccountModel> _accounts = AccountsAccess.LoadAll();

        var account = _accounts.FirstOrDefault(x => x.Id == id);
        if (account == null) throw new Exception($"Account with ID {id}, not found.");

        foreach (var miles in account.Miles)
        {
            String newLevel = CalculateLevel(miles.Experience);
            if (miles.Level != newLevel)
            {
                miles.Level = newLevel;
                miles.History += $"\nLevel updated to {newLevel} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
        }

        AccountsAccess.WriteAll(_accounts);
    }

    public static void UpdateAllAccountLevels()
    {
        List<AccountModel> _accounts = AccountsAccess.LoadAll();


        foreach (var account in _accounts)
        {
            foreach (var miles in account.Miles)
            {
                string newLevel = CalculateLevel(miles.Experience);
                if (miles.Level != newLevel)
                {
                    miles.Level = newLevel;
                    miles.History += $"\nLevel updated to {newLevel} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                }
            }
        }
        AccountsAccess.WriteAll(_accounts);
    }

    public static string DetermineFlightType(int distance) // Returns the type of flight for xp calculation. 
    {
        if (distance <= _short_Max) return "Short";
        if (distance <= _medium_Max) return "Medium";
        return "Long";
    }

    // TODO implement conditional statements for every flight type
    public static string DetermineSeatClass(FlightModel flight, PassengerModel passenger)
    {
        return flight.SeatClassOptions.FirstOrDefault()?.Class ?? "Economy";
    }

    public static int CalculateExperiencePoints(FlightModel flight, string seatClass)
    {
        if (!DateTime.TryParse(flight.DepartureTime, out DateTime departureTime) ||
            departureTime > DateTime.Now)
        {
            return 0;  // No XP for flights that have not occcured.
        }

        string flightType = DetermineFlightType(flight.Distance);

        if (ExperiencePoints.TryGetValue((flightType, seatClass), out int xp))
        {
            return xp;
        }

        return 0; // Return 0 if seatClass not found.
    }

    public static void UpdateFlightExperience(int accountId)
    {
        var accounts = AccountsAccess.LoadAll();
        var bookings = BookingAccess.LoadAll();
        var flights = FlightsAccess.LoadAll();

        var account = accounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null || account.Miles == null || account.Miles.Count == 0)
        {
            throw new ArgumentException($"Account {accountId} not found or has no miles record");
        }

        var milesRecord = account.Miles[0];

        // Get bookings for this user
        var userBookings = bookings.Where(b => b.UserId == accountId).ToList();

        foreach (var booking in userBookings)
        {
            // Find the flight associated with this booking
            var flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
            if (flight == null) continue;

            // Iterate through passengers in the booking
            foreach (var passenger in booking.Passengers)
            {
                // Determine seat class for the passenger
                var seatClass = DetermineSeatClass(flight, passenger); // Update as necessary to match data structure

                // Calculate experience points
                int xp = CalculateExperiencePoints(flight, seatClass);

                if (xp > 0)
                {
                    milesRecord.Experience += xp;
                    milesRecord.History += $"\nEarned {xp} XP from flight {flight.FlightNumber} ({flight.Origin} to {flight.Destination}) - {seatClass} class at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                }
            }
        }

        // Update level after accumulating XP
        milesRecord.Level = CalculateLevel(milesRecord.Experience);

        AccountsAccess.WriteAll(accounts);
    }

    public static void CalculateMilesFromBooking(int accountId)
    {
        var accounts = AccountsAccess.LoadAll();
        var bookings = BookingAccess.LoadAll();

        var account = accounts.FirstOrDefault(a => a.Id == accountId);

        if (account == null || account.Miles == null || account.Miles.Count == 0)
        {
            throw new ArgumentException($"Account {accountId} not found or has no miles record");
        }

        var milesRecord = account.Miles[0];
        var currentLevel = milesRecord.Level;

        // Get bookings for this user
        var userBookings = bookings.Where(b => b.UserId == accountId).ToList();

        int totalMilesEarned = 0;

        foreach (var booking in userBookings)
        {
            int milesMultiplier = currentLevel switch
            {
                "Bronze" => 4,
                "Silver" => 6,
                "Gold" => 7,
                "Platinum" => 8,
                _ => 4 // Default to Bronze rate if level is unexpected
            };

            int bookingMiles = (int)(booking.TotalPrice * milesMultiplier);
            totalMilesEarned += bookingMiles;

            // Update miles history
            milesRecord.History += $"\nEarned {bookingMiles} Miles from booking {booking.BookingId} - {booking.TotalPrice} euros at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
        milesRecord.Points += totalMilesEarned;
        AccountsAccess.WriteAll(accounts);
    }

}
