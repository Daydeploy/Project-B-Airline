public class SeatSelectionUI
{
    private PlaneConfig currentConfig;
    private Dictionary<string, string> occupiedSeats = new Dictionary<string, string>(); // Seat -> Passenger Initials
    private Dictionary<string, string> temporarySeats = new Dictionary<string, string>();
    private Dictionary<string, bool> petSeats = new Dictionary<string, bool>();

    private readonly Dictionary<string, string> planeTypeAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Airbus 330"] = "Airbus A330",
        ["Airbus-330"] = "Airbus A330",
        ["A330"] = "Airbus A330"
    }; // omdat om een of ander reden airbus steeds niet werkt dfus dan maar zo
    
    private readonly Dictionary<string, PlaneConfig> planeConfigs = new Dictionary<string, PlaneConfig>(StringComparer.OrdinalIgnoreCase)
    {
        ["Boeing 737"] = new PlaneConfig //dict met t 
        {
            Rows = 33,
            SeatsPerRow = 6,
            SeatClasses = new[]
            {
                (1, 4),     // First Class
                (5, 12),    // Business Class
                (13, 33)    // Economy Class
            } 
        },
        
        ["Boeing 787"] = new PlaneConfig
        {
            Rows = 38,
            SeatsPerRow = 9,
            SeatClasses = new[]
            {
                (1, 6),     // First Class
                (7, 16),    // Business Class
                (17, 38)    // Economy Class
            }
        },
        
        ["Airbus A330"] = new PlaneConfig
        {
            Rows = 50,
            SeatsPerRow = 9,
            SeatClasses = new[]
            {
                (1, 4),     // First Class
                (5, 14),    // Business Class
                (15, 50)    // Economy Class
            } // tuple met start en eind rij 
        }
    };

    public void LoadExistingBookings(int flightId)
    {
        occupiedSeats.Clear();
        petSeats.Clear();
        
        var bookings = BookingAccess.LoadAll()
            .Where(b => b.FlightId == flightId)
            .ToList();

        foreach (var booking in bookings)
        {
            foreach (var passenger in booking.Passengers)
            {
                if (!string.IsNullOrEmpty(passenger.SeatNumber))
                {
                    occupiedSeats[passenger.SeatNumber] = "■";

                    if (passenger.HasPet)
                    {
                        petSeats[passenger.SeatNumber] = true;
                    }
                }
            }
        }
    }

    private bool AddAisleSpace(int seatIndex)
    {
        switch (currentConfig.SeatsPerRow)
        {
            case 6: 
                return seatIndex == 2; // Boeing 737
            case 9:  
                return seatIndex == 2 || seatIndex == 5; // Boeing 787, Airbus A330
            default:
                return false;
        }
    }

    private int GetTotalAisleSpaces()
    {
        switch (currentConfig.SeatsPerRow)
        {
            case 6:  return 1;
            case 9:  return 2;
            default: return 0;
        }
    }

    public void SetSeatOccupied(string seatNumber, string passengerName = "", bool occupied = true)
    {
        if (occupied)
        {
            string initials = !string.IsNullOrEmpty(passengerName) 
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
        
        if (!int.TryParse(new string(seatNumber.Where(char.IsDigit).ToArray()), out int row))
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
        
        if (!planeConfigs.TryGetValue(planeType, out PlaneConfig planeConfig))
            return string.Empty;
        
        if (!int.TryParse(new string(seatNumber.Where(char.IsDigit).ToArray()), out int row))
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
        {
            if (planeTypeAliases.TryGetValue(planeType, out string resolvedType))
                planeType = resolvedType;
        }

        var config = planeConfigs[planeType];
        var totalSeats = config.Rows * config.SeatsPerRow;

        LoadExistingBookings(flightId);
        return totalSeats - occupiedSeats.Count;
    }

    // Add these new methods
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
        {
            occupiedSeats[seat.Key] = seat.Value;
        }
        temporarySeats.Clear();
    }
    
    public string SelectSeat(string planeType, int flightId, List<PassengerModel> currentPassengers = null)
    {
        // Normalize plane type
        if (planeTypeAliases.TryGetValue(planeType, out string normalizedType))
        {
            planeType = normalizedType; // zodat airbus werkt dus zet je de plane naar de normalizedType
        }

        currentConfig = planeConfigs[planeType];
        LoadExistingBookings(flightId);
        
        if (currentPassengers != null)
        {
            foreach (var passenger in currentPassengers)
            {
                if (!string.IsNullOrEmpty(passenger.SeatNumber))
                {
                    string initials = new string(passenger.Name.Split(' ')
                        .Select(s => s[0])
                        .Take(2)
                        .ToArray());
                    temporarySeats[passenger.SeatNumber] = initials;
                }
            }
        }
        int currentRow = 1;
        int currentSeat = 0;
        bool seatSelected = false;

        while (!seatSelected)
        {
            Console.Clear();
            DisplayLegend();
            DisplayPlane(currentRow, currentSeat, planeType);
            
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentRow > 1) currentRow--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentRow < currentConfig.Rows) currentRow++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentSeat > 0) currentSeat--;
                    break;
                case ConsoleKey.RightArrow:
                    if (currentSeat < currentConfig.SeatsPerRow - 1) currentSeat++;
                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("\nSeat selection cancelled.");
                    return null;
                case ConsoleKey.Enter:
                    string seatNumber = $"{currentRow}{(char)('A' + currentSeat)}";
                    // Check both permanent and temporary seat assignments
                    if (occupiedSeats.ContainsKey(seatNumber) || temporarySeats.ContainsKey(seatNumber))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nThis seat is already occupied! Choose another seat.");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        seatSelected = true;
                        AddTemporarySeat(seatNumber);
                        return seatNumber;
                    }
                    break;
            }
        }

        return null;
    }
    
    private void DisplayLegend()
    {
        Console.WriteLine("\n === Seat Selection === ");
        Console.WriteLine("Use arrow keys to navigate, ENTER to select and ESCAPE to cancel\n");
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("■ First Class  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("■ Business Class  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("■ Economy Class  ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("■ Occupied  ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("▲ With Pet  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("□ Available\n");
        
        Console.Write("    ");
        for (char c = 'A'; c < 'A' + currentConfig.SeatsPerRow; c++)
        {
            Console.Write($" {c} ");
            if (AddAisleSpace(c - 'A'))
            {
                Console.Write("  ");
            }
        }
        Console.WriteLine("\n");
    }

    private void DisplayPlane(int selectedRow, int selectedSeat, string planeType)
    {
        for (int row = 1; row <= currentConfig.Rows; row++)
        {
            Console.Write($" {row,2} |");
            
            for (int seat = 0; seat < currentConfig.SeatsPerRow; seat++)
            {
                string seatNumber = $"{row}{(char)('A' + seat)}";
                bool isSelected = row == selectedRow && seat == selectedSeat;
                bool isOccupied = occupiedSeats.ContainsKey(seatNumber) || temporarySeats.ContainsKey(seatNumber);
                bool hasPet = petSeats.ContainsKey(seatNumber);

                // Set color based on seat class
                if (row <= currentConfig.SeatClasses[0].EndRow)
                    Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
                else if (row <= currentConfig.SeatClasses[1].EndRow)
                    Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

                if (isOccupied)
                {
                    Console.ForegroundColor = hasPet ? ConsoleColor.DarkGray : ConsoleColor.Red;
                }
                
                string displayChar = "□";
                if (isOccupied)
                {
                    displayChar = occupiedSeats.TryGetValue(seatNumber, out string permanent) ? permanent :
                                temporarySeats.TryGetValue(seatNumber, out string temp) ? temp : "■";
                }
                
                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[{displayChar}]");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write($" {displayChar} ");
                }
                
                if (AddAisleSpace(seat))
                {
                    Console.Write("  ");
                }
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |");

            // Add space between classes
            if (row == currentConfig.SeatClasses[0].EndRow || row == currentConfig.SeatClasses[1].EndRow)
                Console.WriteLine("     +" + new string('-', currentConfig.SeatsPerRow * 3 + GetTotalAisleSpaces()) + "+");
        }
        Console.WriteLine($"\nAircraft: {planeType}\n");
    }

}

public class PlaneConfig
{
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public (int StartRow, int EndRow)[] SeatClasses { get; set; }

} 