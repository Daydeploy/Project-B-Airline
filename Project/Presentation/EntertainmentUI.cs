static class EntertainmentUI
{
    private static readonly EntertainmentLogic _entertainmentLogic = new EntertainmentLogic();

    private static void DisplayEntertainmentOptions()
    {
        var entertainmentOptions = EntertainmentDataAccess.LoadAll();
        
        Console.WriteLine("\nAvailable Comfort Packages:");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(new string('-', 80));
        Console.ResetColor();
        
        foreach (var option in entertainmentOptions)
        {
            Console.WriteLine($"{option.Id}. {option.Name} - {string.Join(", ", option.Contents)} (€{option.Cost:F2})");
            Console.WriteLine($"   Available in: {string.Join(", ", option.AvailableIn)}");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(new string('-', 80));
            Console.ResetColor();
        }
    }

    public static void ShowEntertainment()
    {
        int currentUserId = UserLogin.UserAccountServiceLogic.CurrentUserId;
        List<BookingModel> bookedFlights = BookingAccess.LoadAll()
            .Where(b => b.UserId == currentUserId)
            .ToList();

        if (bookedFlights.Count == 0)
        {
            Console.WriteLine("\nYou have no booked flights, or not available for this type of flight.");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            return;
        }

        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("=== Entertainment Options ===\n");
        Console.ResetColor();

        Console.WriteLine("Your Booked Flights:");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(new string('-', 80));
        Console.ResetColor();
    
        foreach (var booking in bookedFlights)
        {
            var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
            if (flight != null)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}");
                System.Console.WriteLine($"Aircraft type: {flight.PlaneType}");
                System.Console.WriteLine($"Seat Class: {flight.SeatClassOptions.FirstOrDefault()?.SeatClass}");
                Console.WriteLine($"Flight: {flight.Origin} to {flight.Destination}");
                Console.WriteLine($"Date: {flight.DepartureTime}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(new string('-', 80));
                Console.ResetColor();
            }
        }
    
        Console.Write("\nEnter Booking ID to purchase entertainment (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int bookingId) || bookingId == 0)
        {
            return;
        }
    
        var selectedBooking = bookedFlights.FirstOrDefault(b => b.BookingId == bookingId);
        if (selectedBooking == null)
        {
            Console.WriteLine("Invalid booking ID.");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            return;
        }
    
        var options = EntertainmentDataAccess.LoadAll();
        if (options == null || !options.Any())
        {
            Console.WriteLine("\nNo entertainment options available.");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            return;
        }
    
        HashSet<int> selectedOptions = new HashSet<int>();
        decimal totalCost = 0;
    
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== Entertainment Selection for Booking {bookingId} ===");
            Console.WriteLine($"Current selections: {selectedOptions.Count}");
            Console.WriteLine($"Total cost: {totalCost:C}\n");
    
            DisplayEntertainmentOptions();
            Console.WriteLine("\nSelected items:");
            foreach (var selectedOptionId in selectedOptions)
            {
                var option = options.First(o => o.Id == selectedOptionId);
                Console.WriteLine($"- {option.Name} ({option.Cost:C})");
            }
    
            Console.WriteLine("\nEnter Package ID to add (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int optionId) || optionId == 0)
            {
                break;
            }
    
            var selectedOption = options.FirstOrDefault(p => p.Id == optionId);
            if (selectedOption == null)
            {
                Console.WriteLine("Invalid option ID.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                continue;
            }
    
            if (selectedOptions.Contains(optionId))
            {
                Console.WriteLine("You've already selected this option.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            selectedOptions.Add(optionId);
            totalCost += selectedOption.Cost;
            Console.WriteLine($"\nAdded {selectedOption.Name}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    
        if (selectedOptions.Any())
        {
            bool allSuccessful = true;
            foreach (var selectedOptionId in selectedOptions)
            {
                bool success = _entertainmentLogic.AddEntertainmentToBooking(bookingId, selectedOptionId);
                if (!success)
                {
                    allSuccessful = false;
                    Console.WriteLine($"\nFailed to add entertainment option {selectedOptionId}");
                }
            }
            
            if (allSuccessful)
            {
                Console.WriteLine("\nAll entertainment options added successfully!");
            }
            else
            {
                Console.WriteLine("\nSome entertainment options could not be added.");
            }
        }
    
        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }
}