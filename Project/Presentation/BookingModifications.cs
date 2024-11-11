public static class BookingModifications
{
    public static void DisplayBookingLegend()
    {
        Console.WriteLine("\nLegend:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Yellow");
        Console.ResetColor();
        Console.WriteLine(" - Booking and Flight Information");
        
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("Cyan");
        Console.ResetColor();
        Console.WriteLine(" - Flight Route and Duration");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Green");
        Console.ResetColor();
        Console.WriteLine(" - Price Information");
        
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Dark Yellow");
        Console.ResetColor();
        Console.WriteLine(" - Seat Information");

        Console.WriteLine("\nCommands:");
        Console.WriteLine("M - Modify booking");
        Console.WriteLine("B - Back to menu");
    }

    public static void ModifyBookingInteractive(int userId)
    {
        Console.Clear();
        var bookings = BookingAccess.LoadAll().Where(b => b.UserId == userId).ToList();
        
        Console.WriteLine("=== Modify Booking ===\n");
        Console.WriteLine("Available bookings:");
        
        for (int i = 0; i < bookings.Count; i++)
        {
            var booking = bookings[i];
            var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
            if (flight == null) continue;

            Console.WriteLine($"\n{i + 1}. Booking ID: {booking.BookingId}");
            Console.WriteLine($"   Flight: {flight.Origin} → {flight.Destination}");
            Console.WriteLine($"   Date: {DateTime.Parse(flight.DepartureTime):dd MMM yyyy}");
        }

        Console.Write("\nEnter booking number to modify (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int bookingChoice) || bookingChoice < 0 || bookingChoice > bookings.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (bookingChoice == 0) return;

        var selectedBooking = bookings[bookingChoice - 1];
        ModifySelectedBooking(selectedBooking);
    }

    private static void ModifySelectedBooking(BookingModel booking)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Modifying Booking {booking.BookingId} ===\n");
            Console.WriteLine("1. Change seat assignment");
            Console.WriteLine("2. Update baggage options");
            Console.WriteLine("3. Modify passenger information");
            Console.WriteLine("4. Back to bookings view");

            Console.Write("\nSelect option (1-4): ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid selection. Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            switch (choice)
            {
                case 1:
                    ModifySeatAssignment(booking);
                    break;
                case 2:
                    ModifyBaggageOptions(booking);
                    break;
                case 3:
                    ModifyPassengerInfo(booking);
                    break;
                case 4:
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    private static void ModifyBooking()
    {
        var bookings = BookingAccess.LoadAll();
        if (!DisplayBookings(bookings)) return;

        int bookingNumber = GetBookingSelection(bookings.Count);
        if (bookingNumber == -1) return;

        var booking = bookings[bookingNumber - 1];
        if (!DisplayPassengers(booking)) return;

        int passengerNumber = GetPassengerSelection(booking.Passengers.Count);
        if (passengerNumber == -1) return;

        ModifyPassengerDetails(booking.FlightId, passengerNumber - 1);
    }

    private static void ModifySeatAssignment(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Change Seat Assignment ===\n");
        
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {booking.Passengers[i].Name} - Current Seat: {booking.Passengers[i].SeatNumber}");
        }

        Console.Write("\nSelect passenger number (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int passengerChoice) || 
            passengerChoice < 0 || 
            passengerChoice > booking.Passengers.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (passengerChoice == 0) return;

        var seatSelector = new SeatSelectionUI();
        // Load existing booked seats
        var existingBookings = BookingLogic.GetBookingsForFlight(booking.FlightId);
        foreach (var existingBooking in existingBookings)
        {
            foreach (var passenger in existingBooking.Passengers)
            {
                if (passenger != booking.Passengers[passengerChoice - 1]) // Don't block current passenger's seat
                {
                    seatSelector.SetSeatOccupied(passenger.SeatNumber);
                }
            }
        }

        Console.WriteLine("\nSelect new seat:");
        string newSeat = seatSelector.SelectSeat();
        
        if (newSeat != null)
        {
            var passenger = booking.Passengers[passengerChoice - 1];
            passenger.SeatNumber = newSeat;
            BookingAccess.WriteAll(BookingAccess.LoadAll());
            Console.WriteLine($"\nSeat successfully changed to {newSeat}");
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ModifyBaggageOptions(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Update Baggage Options ===\n");
        
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {booking.Passengers[i].Name} - Checked Baggage: {(booking.Passengers[i].HasCheckedBaggage ? "Yes" : "No")}");
        }

        Console.Write("\nSelect passenger number (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int passengerChoice) || 
            passengerChoice < 0 || 
            passengerChoice > booking.Passengers.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (passengerChoice == 0) return;

        var passenger = booking.Passengers[passengerChoice - 1];
        Console.Write($"\nToggle checked baggage for {passenger.Name}? (Y/N): ");
        if (Console.ReadLine()?.ToUpper() == "Y")
        {
            passenger.HasCheckedBaggage = !passenger.HasCheckedBaggage;
            BookingAccess.WriteAll(BookingAccess.LoadAll());
            Console.WriteLine($"\nBaggage option updated. Checked baggage is now: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ModifyPassengerInfo(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Modify Passenger Information ===\n");
        
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {booking.Passengers[i].Name}");
        }

        Console.Write("\nSelect passenger number (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int passengerChoice) || 
            passengerChoice < 0 || 
            passengerChoice > booking.Passengers.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (passengerChoice == 0) return;

        var passenger = booking.Passengers[passengerChoice - 1];
        Console.WriteLine($"\nCurrent name: {passenger.Name}");
        Console.Write("Enter new name (or press Enter to keep current): ");
        string newName = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(newName))
        {
            passenger.Name = newName;
            BookingAccess.WriteAll(BookingAccess.LoadAll());
            Console.WriteLine("\nPassenger name updated successfully");
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}

