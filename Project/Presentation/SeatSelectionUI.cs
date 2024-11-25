public class SeatSelectionUI
{
    private PlaneConfig currentConfig;
    private Dictionary<string, bool> occupiedSeats = new Dictionary<string, bool>();
    
    // Add dictionary of plane type variations to handle different Aircrafts
    private readonly Dictionary<string, string> planeTypeAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Airbus 330"] = "Airbus A330",
        ["Airbus-330"] = "Airbus A330",
        ["A330"] = "Airbus A330"
    }; // omdat om een of ander reden airbus steeds niet werkt dfus dan maar zo
    
    private readonly Dictionary<string, PlaneConfig> planeConfigs = new Dictionary<string, PlaneConfig>(StringComparer.OrdinalIgnoreCase)
    {
        ["Boeing 737"] = new PlaneConfig
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
            }
        }
    };


    public string SelectSeat(string planeType)
    {
        // Console.WriteLine($"DEBUG: Received plane type: '{planeType}'");
        // Console.WriteLine($"DEBUG: Exact string comparison with 'Airbus A330': {planeType == "Airbus A330"}");
        // Console.WriteLine($"DEBUG: String length: {planeType.Length}");
        // Console.WriteLine($"DEBUG: Character codes: {string.Join(",", planeType.Select(c => ((int)c)))}");
        // Console.WriteLine($"DEBUG: Available configurations: {string.Join(", ", planeConfigs.Keys)}");

        // Normalize plane type
        if (planeTypeAliases.TryGetValue(planeType, out string normalizedType))
        {
            planeType = normalizedType; // zodat airbus werkt dus zet je de plane naar de normalizedType
        }

        if (!planeConfigs.ContainsKey(planeType))
        {
            throw new ArgumentException($"Unsupported plane type: {planeType}. Available types: {string.Join(", ", planeConfigs.Keys)}");
        }

        currentConfig = planeConfigs[planeType];
        int currentRow = 1;
        int currentSeat = 0;
        bool seatSelected = false;

        while (!seatSelected)
        {
            Console.Clear();
            DisplayLegend();
            DisplayPlane(currentRow, currentSeat);
            
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
                    if (!occupiedSeats.ContainsKey(seatNumber))
                    {
                        return seatNumber;
                    }
                    break;
            }
            Console.WriteLine($"\nAircraft: {planeType}\n");
        }

        return null;
    }
    
    private void DisplayLegend()
    {
        Console.WriteLine("\n === Seat Selection === ");
        Console.WriteLine("Use arrow keys to navigate, ENTER to select and ESCAPE to cancel\n");
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("■ First Class ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("■ Business Class ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("■ Economy Class ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("■ Occupied ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("□ Available\n");
        
        Console.Write("    ");
        for (char c = 'A'; c < 'A' + currentConfig.SeatsPerRow; c++)
        {
            Console.Write($"{c}");
            
            // Add extra space after every third seat (aisle)
            if ((c - 'A' + 1) % 3 == 0 && (c - 'A' + 1) < currentConfig.SeatsPerRow)
            {
                Console.Write("   ");
            }
            else
            {
                Console.Write("   ");
            }
        }
        Console.WriteLine("\n");
    }

    private void DisplayPlane(int selectedRow, int selectedSeat)
    {
        for (int row = 1; row <= currentConfig.Rows; row++)
        {
            Console.Write($" {row,2} |");
            
            for (int seat = 0; seat < currentConfig.SeatsPerRow; seat++)
            {
                string seatNumber = $"{row}{(char)('A' + seat)}";
                bool isSelected = row == selectedRow && seat == selectedSeat;
                bool isOccupied = occupiedSeats.ContainsKey(seatNumber);

                // Set color based on seat class
                if (row <= currentConfig.SeatClasses[0].EndRow)
                    Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
                else if (row <= currentConfig.SeatClasses[1].EndRow)
                    Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

                if (isOccupied)
                    Console.ForegroundColor = ConsoleColor.Red;

                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("[■]");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write(isOccupied ? " ■ " : " □ ");
                }

                // Add aisle space based on plane type
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

    public void SetSeatOccupied(string seatNumber, bool occupied = true)
    {
        if (occupied)
            occupiedSeats[seatNumber] = true;
        else
            occupiedSeats.Remove(seatNumber);
    }

    public string GetSeatClass(string seatNumber)
    {
        if (currentConfig == null)
            throw new InvalidOperationException("Plane configuration not set. Call SelectSeat first.");
            
        int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));
        
        if (row <= currentConfig.SeatClasses[0].EndRow)
            return "First";
        if (row <= currentConfig.SeatClasses[1].EndRow)
            return "Business";
        return "Economy";
    }

    public void SelectPetsForBooking(int bookingId)
    {
        var petService = new PetService();
        List<PetModel> petsToBook = new List<PetModel>();

        while (true)
        {
            Console.WriteLine("Enter pet type (Dog, Cat, Other) or 'done' to finish:");
            string petType = Console.ReadLine();
            if (petType.Equals("done", StringComparison.OrdinalIgnoreCase)) break;

            string seatingLocation;
            if (petType.Equals("Dog", StringComparison.OrdinalIgnoreCase) || 
                petType.Equals("Cat", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Would you like to place the pet in the seat next to you? (yes/no)");
                string response = Console.ReadLine();
                seatingLocation = response.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "Seat" : "Luggage Room";
            }
            else
            {
                seatingLocation = "Luggage Room";
            }

            var pet = new PetModel
            {
                Type = petType,
                Size = "Medium",
                SeatingLocation = seatingLocation,
                Color = petType == "Dog" ? "Brown" : "Gray"
            };

            petsToBook.Add(pet);
            petService.AddPetToBooking(bookingId, pet);
        }
    }
}

public class PlaneConfig
{
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public (int StartRow, int EndRow)[] SeatClasses { get; set; }

}