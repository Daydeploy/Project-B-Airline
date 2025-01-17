public class SeatSelectionUI
{
    private readonly SeatSelectionLogic seatSelectionLogic;

    public SeatSelectionUI(SeatSelectionLogic seatSelectionLogic)
    {
        this.seatSelectionLogic = seatSelectionLogic;
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
        for (var c = 'A'; c < 'A' + seatSelectionLogic.CurrentConfig.SeatsPerRow; c++)
        {
            Console.Write($" {c} ");
            if (seatSelectionLogic.AddAisleSpace(c - 'A')) Console.Write("  ");
        }

        Console.WriteLine("\n");
    }

    private void DisplayPlane(int selectedRow, int selectedSeat, string planeType)
    {
        var config = seatSelectionLogic.CurrentConfig;

        for (var row = 1; row <= config.Rows; row++)
        {
            Console.Write($" {row,2} |");

            for (var seat = 0; seat < config.SeatsPerRow; seat++)
            {
                var seatNumber = $"{row}{(char)('A' + seat)}";
                var isSelected = row == selectedRow && seat == selectedSeat;
                var isOccupied = !seatSelectionLogic.IsSeatAvailable(seatNumber);
                var hasPet = seatSelectionLogic.PetSeats.ContainsKey(seatNumber);

                // Set color based on seat class
                if (row <= config.SeatClasses[0].EndRow)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else if (row <= config.SeatClasses[1].EndRow)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;

                if (isOccupied)
                    Console.ForegroundColor = hasPet ? ConsoleColor.DarkGray : ConsoleColor.Red;

                var displayChar = "□";
                if (isOccupied)
                {
                    if (seatSelectionLogic.OccupiedSeats.TryGetValue(seatNumber, out var permanent))
                        displayChar = permanent;
                    else if (seatSelectionLogic.TemporarySeats.TryGetValue(seatNumber, out var temp))
                        displayChar = temp;
                    else
                        displayChar = "■";
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

                if (seatSelectionLogic.AddAisleSpace(seat)) Console.Write("  ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |");

            if (row == config.SeatClasses[0].EndRow || row == config.SeatClasses[1].EndRow)
                Console.WriteLine("     +" + new string('-', config.SeatsPerRow * 3 + seatSelectionLogic.GetTotalAisleSpaces()) + "+");
        }

        Console.WriteLine($"\nAircraft: {planeType}\n");
    }

    public string SelectSeat(string planeType, int flightId, List<PassengerModel> currentPassengers = null)
    {
        seatSelectionLogic.SetPlaneType(planeType);
        seatSelectionLogic.LoadExistingBookings(flightId);

        if (currentPassengers != null)
        {
            foreach (var passenger in currentPassengers)
            {
                if (!string.IsNullOrEmpty(passenger.SeatNumber))
                {
                    var initials = new string(passenger.Name.Split(' ')
                        .Select(s => s[0])
                        .Take(2)
                        .ToArray());
                    seatSelectionLogic.AddTemporarySeat(passenger.SeatNumber, initials);
                }
            }
        }

        var currentRow = 1;
        var currentSeat = 0;
        var seatSelected = false;

        while (!seatSelected)
        {
            Console.Clear();
            DisplayLegend();
            DisplayPlane(currentRow, currentSeat, planeType);

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentRow > 1) currentRow--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentRow < seatSelectionLogic.CurrentConfig.Rows) currentRow++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentSeat > 0) currentSeat--;
                    break;
                case ConsoleKey.RightArrow:
                    if (currentSeat < seatSelectionLogic.CurrentConfig.SeatsPerRow - 1) currentSeat++;
                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("\nSeat selection cancelled.");
                    return null;
                case ConsoleKey.Enter:
                    var seatNumber = $"{currentRow}{(char)('A' + currentSeat)}";
                    if (!seatSelectionLogic.IsSeatAvailable(seatNumber))
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
                        seatSelectionLogic.AddTemporarySeat(seatNumber);
                        return seatNumber;
                    }
                    break;
            }
        }

        return null;
    }
}