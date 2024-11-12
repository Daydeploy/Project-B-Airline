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

        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found for modification. Press any key to return...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("=== Modify Booking ===\n");

        // Create options array for MenuNavigationService
        string[] options = new string[bookings.Count + 1];
        for (int i = 0; i < bookings.Count; i++)
        {
            var booking = bookings[i];
            var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
            if (flight != null)
            {
                options[i] =
                    $"Booking ID: {booking.BookingId,-5} | Route: {flight.Origin,-10} → {flight.Destination,-15} | Departure: {DateTime.Parse(flight.DepartureTime):dd MMM yyyy, HH:mm}";
                ;
                ;
            }
        }

        options[bookings.Count] = "Cancel"; // Add an option to cancel

        // Use MenuNavigationService to select a booking
        int selectedBookingIndex = MenuNavigationService.NavigateMenu(options, "Select a booking to modify:");

        if (selectedBookingIndex == -1 || selectedBookingIndex == bookings.Count)
        {
            Console.WriteLine("Operation canceled. Press any key to return...");
            Console.ReadKey();
            return;
        }

        // Modify the selected booking
        var selectedBooking = bookings[selectedBookingIndex];
        ModifySelectedBooking(selectedBooking);
    }


    private static void ModifySelectedBooking(BookingModel booking)
    {
        string[] options =
        {
            "Change seat assignment",
            "Update baggage options",
            "Modify passenger information",
            "Back to bookings view"
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Modifying Booking {booking.BookingId} ===\n");

            // Use MenuNavigationService to navigate options
            int selectedOption = MenuNavigationService.NavigateMenu(options, "Select an option:");

            switch (selectedOption)
            {
                case 0:
                    ModifySeatAssignment(booking);
                    break;
                case 1:
                    ModifyBaggageOptions(booking);
                    break;
                case 2:
                    ModifyPassengerInfo(booking);
                    break;
                case 3:
                    return; // Exit to bookings view
                default:
                    Console.WriteLine("Invalid option. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    // private static void ModifyBooking()
    // {
    //     var bookings = BookingAccess.LoadAll();
    //     if (!DisplayBookings(bookings)) return;

    //     int bookingNumber = GetBookingSelection(bookings.Count);
    //     if (bookingNumber == -1) return;

    //     var booking = bookings[bookingNumber - 1];
    //     if (!DisplayPassengers(booking)) return;

    //     int passengerNumber = GetPassengerSelection(booking.Passengers.Count);
    //     if (passengerNumber == -1) return;

    //     ModifyPassengerDetails(booking.FlightId, passengerNumber - 1);
    // }


    private static void ModifySeatAssignment(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Change Seat Assignment ===\n");

        // Create an options array for each passenger
        string[] passengerOptions = new string[booking.Passengers.Count + 1];
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            passengerOptions[i] = $"{booking.Passengers[i].Name} - Current Seat: {booking.Passengers[i].SeatNumber}";
        }

        passengerOptions[booking.Passengers.Count] = "Cancel"; // Add a cancel option

        // Use MenuNavigationService to select a passenger
        int selectedPassengerIndex =
            MenuNavigationService.NavigateMenu(passengerOptions, "Select a passenger to change seat:");

        // Check if the user selected "Cancel"
        if (selectedPassengerIndex == -1 || selectedPassengerIndex == booking.Passengers.Count)
        {
            Console.WriteLine("Operation canceled. Press any key to return...");
            Console.ReadKey();
            return;
        }

        var seatSelector = new SeatSelectionUI();
        // Load existing booked seats
        var existingBookings = BookingLogic.GetBookingsForFlight(booking.FlightId);
        foreach (var existingBooking in existingBookings)
        {
            foreach (var passenger in existingBooking.Passengers)
            {
                if (passenger != booking.Passengers[passengerChoice - 1]) // Skip the current passenger
                {
                    seatSelector.SetSeatOccupied(passenger.SeatNumber);
                }
            }
        }

        // Console.WriteLine("\nSelect new seat:");
        string newSeat = seatSelector.SelectSeat();

        if (!string.IsNullOrEmpty(newSeat))
        {
            var passenger = booking.Passengers[selectedPassengerIndex];
            passenger.SeatNumber = newSeat;
            UserLogin.ModifyPassengerDetails(booking.FlightId, passengerChoice -1);	
            Console.WriteLine($"\nSeat successfully changed to {newSeat}");
        }
        else
        {
            Console.WriteLine("Seat selection canceled.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }


    private static void ModifyBaggageOptions(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Update Baggage Options ===\n");

        // Create options array for each passenger
        string[] passengerOptions = new string[booking.Passengers.Count + 1];
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            passengerOptions[i] =
                $"{booking.Passengers[i].Name} - Checked Baggage: {(booking.Passengers[i].HasCheckedBaggage ? "Yes" : "No")}";
        }

        passengerOptions[booking.Passengers.Count] = "Cancel"; // Add a cancel option

        // Use MenuNavigationService to select a passenger
        int selectedPassengerIndex =
            MenuNavigationService.NavigateMenu(passengerOptions, "Select a passenger to update baggage:");

        // Check if the user selected "Cancel"
        if (selectedPassengerIndex == -1 || selectedPassengerIndex == booking.Passengers.Count)
        {
            Console.WriteLine("Operation canceled. Press any key to return...");
            Console.ReadKey();
            return;
        }

        var passenger = booking.Passengers[selectedPassengerIndex];
        Console.Write($"\nToggle checked baggage for {passenger.Name}? (Y/N): ");
        if (Console.ReadLine()?.ToUpper() == "Y")
        {
            passenger.HasCheckedBaggage = !passenger.HasCheckedBaggage;
            BookingAccess.WriteAll(BookingAccess.LoadAll()); // Save the updated booking data
            Console.WriteLine(
                $"\nBaggage option updated. Checked baggage is now: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
        }
        else
        {
            Console.WriteLine("\nNo changes made to baggage option.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }


    private static void ModifyPassengerInfo(BookingModel booking)
    {
        Console.Clear();
        Console.WriteLine("=== Modify Passenger Information ===\n");

        // Create options array for each passenger
        string[] passengerOptions = new string[booking.Passengers.Count + 1];
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            passengerOptions[i] = $"{booking.Passengers[i].Name}";
        }

        passengerOptions[booking.Passengers.Count] = "Cancel"; // Add a cancel option

        // Use MenuNavigationService to select a passenger
        int selectedPassengerIndex =
            MenuNavigationService.NavigateMenu(passengerOptions, "Select a passenger to update:");

        // Check if the user selected "Cancel"
        if (selectedPassengerIndex == -1 || selectedPassengerIndex == booking.Passengers.Count)
        {
            Console.WriteLine("Operation canceled. Press any key to return...");
            Console.ReadKey();
            return;
        }

        var passenger = booking.Passengers[selectedPassengerIndex];
        Console.WriteLine($"\nCurrent name: {passenger.Name}");
        Console.Write("Enter new name (or press Enter to keep current): ");
        string newName = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(newName))
        {
            passenger.Name = newName;
            BookingAccess.WriteAll(BookingAccess.LoadAll()); // Save the updated booking data
            Console.WriteLine("\nPassenger name updated successfully");
        }
        else
        {
            Console.WriteLine("\nNo changes made to the passenger name.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}