public class SeatSelectionLogic
{
    private readonly Dictionary<string, PlaneConfig> planeConfigs =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Boeing 737"] = new()
            {
                Rows = 33,
                SeatsPerRow = 6,
                SeatClasses = new[]
                {
                    (1, 4),  // First Class
                    (5, 12), // Business Class
                    (13, 33) // Economy Class
                }
            },

            ["Boeing 787"] = new()
            {
                Rows = 38,
                SeatsPerRow = 9,
                SeatClasses = new[]
                {
                    (1, 6),   // First Class
                    (7, 16),  // Business Class
                    (17, 38)  // Economy Class
                }
            },

            ["Airbus A330"] = new()
            {
                Rows = 50,
                SeatsPerRow = 9,
                SeatClasses = new[]
                {
                    (1, 4),   // First Class
                    (5, 14),  // Business Class
                    (15, 50)  // Economy Class
                }
            }
        };

    private readonly Dictionary<string, string> planeTypeAliases =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Airbus 330"] = "Airbus A330",
            ["Airbus-330"] = "Airbus A330",
            ["A330"] = "Airbus A330"
        };

    private PlaneConfig currentConfig;
    private readonly Dictionary<string, string> occupiedSeats = new();
    private readonly Dictionary<string, bool> petSeats = new();
    private readonly Dictionary<string, string> temporarySeats = new();

    public PlaneConfig CurrentConfig => currentConfig;
    public IReadOnlyDictionary<string, string> OccupiedSeats => occupiedSeats;
    public IReadOnlyDictionary<string, bool> PetSeats => petSeats;
    public IReadOnlyDictionary<string, string> TemporarySeats => temporarySeats;

    public void LoadExistingBookings(int flightId)
    {
        occupiedSeats.Clear();
        petSeats.Clear();

        var bookings = BookingAccess.LoadAll()
            .Where(b => b.FlightId == flightId)
            .ToList();

        foreach (var booking in bookings)
            foreach (var passenger in booking.Passengers)
                if (!string.IsNullOrEmpty(passenger.SeatNumber))
                {
                    occupiedSeats[passenger.SeatNumber] = "■";
                    if (passenger.HasPet) petSeats[passenger.SeatNumber] = true;
                }
    }

    public bool IsSeatAvailable(string seatNumber)
    {
        return !occupiedSeats.ContainsKey(seatNumber) && !temporarySeats.ContainsKey(seatNumber);
    }

    public bool AddAisleSpace(int seatIndex)
    {
        if (currentConfig == null) return false;

        switch (currentConfig.SeatsPerRow)
        {
            case 6:
                return seatIndex == 2;
            case 9:
                return seatIndex == 2 || seatIndex == 5;
            default:
                return false;
        }
    }

    public int GetTotalAisleSpaces()
    {
        if (currentConfig == null) return 0;

        switch (currentConfig.SeatsPerRow)
        {
            case 6: return 1;
            case 9: return 2;
            default: return 0;
        }
    }

    public void SetSeatOccupied(string seatNumber, string passengerName = "", bool occupied = true)
    {
        if (occupied)
        {
            var initials = !string.IsNullOrEmpty(passengerName)
                ? new string(passengerName.Split(' ').Select(s => s[0]).Take(2).ToArray()).ToUpper()
                : "■";
            occupiedSeats[seatNumber] = initials;
        }
        else
        {
            occupiedSeats.Remove(seatNumber);
        }
    }

    public void SetPetSeat(string seatNumber, bool hasPet = true)
    {
        if (hasPet)
            petSeats[seatNumber] = true;
        else
            petSeats.Remove(seatNumber);
    }

    public string GetSeatClass(string seatNumber)
    {
        if (currentConfig == null)
            return string.Empty;

        if (!int.TryParse(new string(seatNumber.Where(char.IsDigit).ToArray()), out var row))
            return string.Empty;

        if (row <= currentConfig.SeatClasses[0].EndRow)
            return "First";
        if (row <= currentConfig.SeatClasses[1].EndRow)
            return "Business";
        return "Economy";
    }

    public string GetSeatClass(string seatNumber, string planeType)
    {
        if (string.IsNullOrEmpty(seatNumber) || string.IsNullOrEmpty(planeType))
            return string.Empty;

        if (!planeConfigs.TryGetValue(planeType, out var planeConfig))
            return string.Empty;

        if (!int.TryParse(new string(seatNumber.Where(char.IsDigit).ToArray()), out var row))
            return string.Empty;

        var (firstStart, firstEnd) = planeConfig.SeatClasses[0];
        var (businessStart, businessEnd) = planeConfig.SeatClasses[1];

        if (row >= firstStart && row <= firstEnd)
            return "First";
        if (row >= businessStart && row <= businessEnd)
            return "Business";

        return "Economy";
    }

    public int GetAvailableSeatsCount(string planeType, int flightId)
    {
        if (!planeConfigs.ContainsKey(planeType))
            if (planeTypeAliases.TryGetValue(planeType, out var resolvedType))
                planeType = resolvedType;

        var config = planeConfigs[planeType];
        var totalSeats = config.Rows * config.SeatsPerRow;

        LoadExistingBookings(flightId);
        return totalSeats - occupiedSeats.Count;
    }

    public void AddTemporarySeat(string seatNumber, string passengerName = "□")
    {
        temporarySeats[seatNumber] = passengerName;
    }

    public void ClearTemporarySeats()
    {
        temporarySeats.Clear();
    }

    public void CommitTemporarySeats()
    {
        foreach (var seat in temporarySeats)
            occupiedSeats[seat.Key] = seat.Value;

        temporarySeats.Clear();
    }

    public void SetPlaneType(string planeType)
    {
        if (planeTypeAliases.TryGetValue(planeType, out var normalizedType))
            planeType = normalizedType;

        if (planeConfigs.TryGetValue(planeType, out var config))
            currentConfig = config;
    }

    public bool IsValidPlaneType(string planeType)
    {
        return planeConfigs.ContainsKey(planeType) || planeTypeAliases.ContainsKey(planeType);
    }
}

public class PlaneConfig
{
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public (int StartRow, int EndRow)[] SeatClasses { get; set; }
}