static class PackagesUI
{
    private static readonly ComfortPackageServiceLogic _comfortPackageServiceLogic = new ComfortPackageServiceLogic();

    private static void DisplayPackageOptions()
    {
        var packages = ComfortPackageDataAccess.LoadAll();

        Console.WriteLine("\nAvailable Comfort Packages:");
        Console.WriteLine(new string('-', 50));

        foreach (var package in packages)
        {
            Console.WriteLine(
                $"{package.Id}. {package.Name} - {string.Join(", ", package.Contents)} (€{package.Cost:F2})");
            Console.WriteLine($"   Available in: {string.Join(", ", package.AvailableIn)}");
            Console.WriteLine(new string('-', 50));
        }
    }

    public static void ShowPackages()
    {
        // Get current user ID
        int currentUserId = UserLogin.UserAccountServiceLogic.CurrentUserId;

        // Use BookingAccess instead of BookingDataAccess
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
        Console.WriteLine("=== Comfort Packages ===\n");
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

        Console.Write("\nEnter Booking ID to purchase a comfort package (0 to cancel): ");
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

        DisplayPackageOptions();
        var packages = ComfortPackageDataAccess.LoadAll();
        if (packages == null || !packages.Any())
        {
            Console.WriteLine("\nNo packages available.");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            return;
        }

        Console.Write("\nEnter Package ID to purchase (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int packageId) || packageId == 0)
        {
            return;
        }

        if (!packages.Any(p => p.Id == packageId))
        {
            Console.WriteLine("Invalid package ID.");
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey();
            return;
        }

        var (success, error) = _comfortPackageServiceLogic.AddPackageToBooking(bookingId, packageId);
        if (success)
        {
            Console.WriteLine("\nComfort package added successfully!");
        }
        else
        {
            Console.WriteLine("\nFailed to add comfort package.");
        }

        Console.WriteLine("\nPress any key to return to menu...");
        Console.ReadKey();
    }
}