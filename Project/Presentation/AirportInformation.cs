internal static class AirportInformation
{
    public static void ViewAirportInformation()
    {
        while (true)
        {
            Console.Clear();
            var airportLogic = new AirportLogic();
            var airports = airportLogic.GetAllAirports();

            var currentIndex = 0;
            DisplayCurrentAirport();

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (currentIndex < airports.Count - 1)
                        {
                            currentIndex++;
                            DisplayCurrentAirport();
                        }

                        break;

                    case ConsoleKey.LeftArrow:
                        if (currentIndex > 0)
                        {
                            currentIndex--;
                            DisplayCurrentAirport();
                        }

                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }

            void DisplayCurrentAirport()
            {
                Console.Clear();
                Console.WriteLine("Navigate through airports using the following commands:");
                Console.WriteLine("ESC - Return to main menu");
                Console.WriteLine("← → Arrow keys - Navigate between airports");
                Console.WriteLine(new string('─', Console.WindowWidth - 1));

                Console.WriteLine($"\nAirport {currentIndex + 1} of {airports.Count}");
                AirportUI.DisplayAirportDetails(airports[currentIndex]);
            }
        }
    }

    // Displays a list of all available destinations
    public static void BrowseDestinations()
    {
        while (true)
        {
            Console.Clear();
            var flightsLogic = new FlightsLogic();
            var destinations = flightsLogic.GetAllDestinations().Distinct().ToList();

            var currentIndex = 0;
            DisplayCurrentDestination();

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (currentIndex < destinations.Count - 1)
                        {
                            currentIndex++;
                            DisplayCurrentDestination();
                        }

                        break;

                    case ConsoleKey.LeftArrow:
                        if (currentIndex > 0)
                        {
                            currentIndex--;
                            DisplayCurrentDestination();
                        }

                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }

            void DisplayCurrentDestination()
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== Browse Destinations ===\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Navigation Controls:");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("← → - Navigate between destinations");
                Console.WriteLine("ESC - Return to main menu\n");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(new string('═', Console.WindowWidth - 1));

                var currentDestination = destinations[currentIndex];
                var flightCount = flightsLogic.GetAllFlights()
                    .Count(f => f.Destination.Equals(currentDestination, StringComparison.OrdinalIgnoreCase));

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\nDestination {currentIndex + 1} of {destinations.Count}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n╔══ {currentDestination} ══╗");

                Console.ForegroundColor = flightCount > 0 ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"\nNumber of flights available: {flightCount}");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\nTo view detailed flight information, please use the Flight Search option.");

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(new string('═', Console.WindowWidth - 1));

                Console.ResetColor();
            }
        }
    }

    public static void EditAirportInformation()
    {
        Console.Clear();
        Console.CursorVisible = false;
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        // Convert airports to menu items
        var airportMenuItems = airports
            .Select(a => $"{a.Name} ({a.Code})")
            .ToArray();

        // Use MenuNavigationService for airport selection
        Console.WriteLine("=== Edit Airport Information ===\n");
        var selectedIndex = MenuNavigationServiceLogic.NavigateMenu(airportMenuItems, "Select Airport to Edit");

        if (selectedIndex == -1) return; // User pressed ESC

        var selectedAirport = airports[selectedIndex];
        var originalAirport = new AirportModel(
            selectedAirport.AirportID,
            selectedAirport.Country,
            selectedAirport.City,
            selectedAirport.Name,
            selectedAirport.Code,
            selectedAirport.Type,
            selectedAirport.PhoneNumber,
            selectedAirport.Address
        );

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Editing: {selectedAirport.Name}\n");

            string[] editOptions =
            {
                $"Country: {selectedAirport.Country}",
                $"City: {selectedAirport.City}",
                $"Name: {selectedAirport.Name}",
                $"Code: {selectedAirport.Code}",
                $"Type: {selectedAirport.Type}",
                $"Phone: {selectedAirport.PhoneNumber}",
                $"Address: {selectedAirport.Address}",
                "Save Changes",
                "Back to Menu"
            };

            var selectedOption = MenuNavigationServiceLogic.NavigateMenu(editOptions, "Select field to edit:");
            if (selectedOption == -1 || selectedOption == editOptions.Length - 1)
            {
                Console.WriteLine("Returning to menu...");
                return;
            }

            if (selectedOption == editOptions.Length - 2)
            {
                // Show changes summary
                Console.Clear();
                Console.WriteLine("=== Changes Summary ===\n");
                if (originalAirport.Country != selectedAirport.Country)
                    Console.WriteLine($"Country: {originalAirport.Country} -> {selectedAirport.Country}");
                if (originalAirport.City != selectedAirport.City)
                    Console.WriteLine($"City: {originalAirport.City} -> {selectedAirport.City}");
                if (originalAirport.Name != selectedAirport.Name)
                    Console.WriteLine($"Name: {originalAirport.Name} -> {selectedAirport.Name}");
                if (originalAirport.Code != selectedAirport.Code)
                    Console.WriteLine($"Code: {originalAirport.Code} -> {selectedAirport.Code}");
                if (originalAirport.Type != selectedAirport.Type)
                    Console.WriteLine($"Type: {originalAirport.Type} -> {selectedAirport.Type}");
                if (originalAirport.PhoneNumber != selectedAirport.PhoneNumber)
                    Console.WriteLine($"Phone: {originalAirport.PhoneNumber} -> {selectedAirport.PhoneNumber}");
                if (originalAirport.Address != selectedAirport.Address)
                    Console.WriteLine($"Address: {originalAirport.Address} -> {selectedAirport.Address}");

                Console.Write("\nSave these changes? (Y/N): ");
                if (Console.ReadLine()?.ToUpper() == "Y")
                {
                    var success = airportLogic.UpdateAirport(selectedAirport);
                    if (success)
                        Console.WriteLine("Airport information updated successfully!");
                    else
                        Console.WriteLine("Error: Failed to update airport information.");

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                continue;
            }

            Console.Write($"\nEnter new {editOptions[selectedOption].Split(':')[0]} (ESC to cancel): ");

            ConsoleKeyInfo key;
            var newValue = "";
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nCanceled. Press any key to continue...");
                    Console.ReadKey();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace && newValue.Length > 0)
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                    Console.Write($"Enter new {editOptions[selectedOption].Split(':')[0]} (ESC to cancel): {newValue}");
                }
                else if (key.KeyChar >= 32)
                {
                    newValue += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            if (!string.IsNullOrWhiteSpace(newValue))
                switch (selectedOption)
                {
                    case 0:
                        selectedAirport.Country = newValue;
                        break;
                    case 1:
                        selectedAirport.City = newValue;
                        break;
                    case 2:
                        selectedAirport.Name = newValue;
                        break;
                    case 3:
                        selectedAirport.Code = newValue;
                        break;
                    case 4:
                        selectedAirport.Type = newValue;
                        break;
                    case 5:
                        selectedAirport.PhoneNumber = newValue;
                        break;
                    case 6:
                        selectedAirport.Address = newValue;
                        break;
                }
        }
    }

    public static void AddNewAirport()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Airport ===\n");
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        Console.Write("Enter country: ");
        var country = Console.ReadLine();

        Console.Write("Enter city: ");
        var city = Console.ReadLine();

        Console.Write("Enter airport name: ");
        var name = Console.ReadLine();

        Console.Write("Enter airport code (e.g., LHR): ");
        var code = Console.ReadLine()?.ToUpper();

        // er word check gedaan of airport al bestaat gebasseerd op de airport code
        if (airports.Any(a => a.Code == code))
        {
            Console.WriteLine($"\nError: Airport with code {code} already exists.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter type (Public/Private): ");
        var type = Console.ReadLine();

        Console.Write("Enter phone number: ");
        var phoneNumber = Console.ReadLine();

        Console.Write("Enter address: ");
        var address = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(country) ||
            string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(code))
        {
            Console.WriteLine("\nError: Country, city, name, and code are required fields.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var newId = airports.Count > 0 ? airports.Max(a => a.AirportID) + 1 : 1;

        var newAirport = new AirportModel(
            newId,
            country,
            city,
            name,
            code,
            type,
            phoneNumber,
            address
        );

        airports.Add(newAirport);
        if (AirportAccess.WriteAllAirports(airports))
            Console.WriteLine("\nAirport added successfully!");
        else
            Console.WriteLine("\nError: Failed to add airport.");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}