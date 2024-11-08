using System;
using System.Collections.Generic;
using System.Linq;

static class Menu
{
    static private UserAccountService _userAccountService = new UserAccountService();

    static public void Start()
    {
        Console.CursorVisible = false; // Hide the cursor
        string[] menuItems =
        {
            "Login",
            "Create Account",
            "Show available Flights",
            "View Available Flights by Destination",
            "Filter Flights by Price",
            "Show Seat Upgrade Options",
            "Exit"
        };

        bool exit = false;

        while (!exit)
        {
            DisplayMenu(menuItems, "Main Menu");
            System.Console.WriteLine(@"
 --------------------------------------------------------------------------------------------------------------------------------------------------------
|                                                                                                                                                        |
| d8888b.  .d88b.  d888888b d888888b d88888b d8888b. d8888b.  .d8b.  .88b  d88.       .d8b.  d888888b d8888b. db      d888888b d8b   db d88888b .d8888.  |
| 88  `8D .8P  Y8. `~~88~~' `~~88~~' 88'     88  `8D 88  `8D d8' `8b 88'YbdP`88      d8' `8b   `88'   88  `8D 88        `88'   888o  88 88'     88'  YP  | 
| 88oobY' 88    88    88       88    88ooooo 88oobY' 88   88 88ooo88 88  88  88      88ooo88    88    88oobY' 88         88    88V8o 88 88ooooo `8bo.    |
| 88`8b   88    88    88       88    88~~~~~ 88`8b   88   88 88~~~88 88  88  88      88~~~88    88    88`8b   88         88    88 V8o88 88~~~~~   `Y8b.  | 
| 88 `88. `8b  d8'    88       88    88.     88 `88. 88  .8D 88   88 88  88  88      88   88   .88.   88 `88. 88booo.   .88.   88  V888 88.     db   8D  |
| 88   YD  `Y88P'     YP       YP    Y88888P 88   YD Y8888D' YP   YP YP  YP  YP      YP   YP Y888888P 88   YD Y88888P Y888888P VP   V8P Y88888P `8888Y'  |
|                                                                                                                                                        |
 --------------------------------------------------------------------------------------------------------------------------------------------------------
");
            int selectedIndex = NavigateMenu(menuItems, "Main Menu");
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    // Reusable DisplayMenu method with a title
    static public void DisplayMenu(string[] menuItems, string title = "")
    {
        Console.Clear();

        if (!string.IsNullOrEmpty(title))
        {
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length)); // Underline the title
        }

        for (int i = 0; i < menuItems.Length; i++)
        {
            Console.WriteLine(menuItems[i]);
            Console.ResetColor(); // Reset the colors
        }
    }

    static public int NavigateMenu(string[] options, string title = "")
    {
        int selectedIndex = 0;

        while (true)
        {
            // Clear and display the menu with highlight
            Console.Clear();
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine(title);
                Console.WriteLine(new string('-', title.Length));
            }

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan; // Highlight color
                    Console.WriteLine($"{options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{options[i]}");
                }
            }

            // Handle key input for navigation
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    return selectedIndex;
            }
        }
    }

    static private void HandleSelection(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "Login":
                UserLogin.Start();
                break;
            case "Create Account":
                CreateAccount();
                break;
            case "Show available Flights":
                UserLogin.ShowAvailableFlights();
                break;
            case "View Available Flights by Destination":
                ShowDestinations();
                break;
            case "Filter Flights by Price":
                FilterFlightsByPriceUI();
                break;
            case "Exit":
                exit = true;
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }

        if (!exit)
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }

    static private void CreateAccount()
    {
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.\n");
        Console.WriteLine("Create a new account");

        Console.WriteLine("Enter your first name:");
        string firstName = Console.ReadLine();
        Console.WriteLine("Enter your last name:");
        string lastName = Console.ReadLine();

        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();

        string password = "";
        string confirmPassword = "";
        bool showPassword = false;

        Console.Write("Enter your password: ");
        password = UserLogin.ReadPassword(ref showPassword);

        Console.Write("Confirm your password: ");
        confirmPassword = UserLogin.ReadPassword(ref showPassword);

        if (password != confirmPassword)
        {
            Console.WriteLine("\nPasswords do not match. Please try again.");
            return;
        }

        Console.WriteLine("Enter your date of birth (yyyy-mm-dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            return;
        }

        bool accountCreated = _userAccountService.CreateAccount(email, password, firstName, lastName, dateOfBirth);

        Console.WriteLine(accountCreated
            ? "Account created successfully. Please login."
            : "Failed to create account. Email may already be in use.");
    }

    static public void ShowDestinations()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports()
            .Where(a => a.Type == "Public" && a.City != "Rotterdam")
            .ToList();

        var validAirports = new List<AirportModel>();
        var flightDictionary = new Dictionary<int, FlightModel>();

        var availableFlights = FlightsLogic.AvailableFlights;

        foreach (var airport in airports)
        {
            var flightAvailable = availableFlights.FirstOrDefault(f => f.Destination == airport.City);
            if (flightAvailable != null)
            {
                validAirports.Add(airport);
                flightDictionary[airport.AirportID] = flightAvailable;
            }
        }

        int selectedIndex = NavigateMenu(
            validAirports.Select(a => $"{a.AirportID}. {a.City}, {a.Country} - {a.Name}").ToArray(),
            "Available Destinations with Flights");

        var selectedAirport = validAirports[selectedIndex];
        Console.Clear();
        Console.WriteLine($"\nYou selected: {selectedAirport.City}, {selectedAirport.Country}");

        if (flightDictionary.TryGetValue(selectedAirport.AirportID, out var flight))
        {
            Console.WriteLine($"Flight from {flight.Origin} to {flight.Destination}");

            DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
            DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);

            Console.WriteLine($"Departure: {departureDateTime:dddd, dd MMMM yyyy HH:mm}");
            Console.WriteLine($"Arrival: {arrivalDateTime:dddd, dd MMMM yyyy HH:mm}");

            Console.WriteLine("Available Prices:");
            foreach (var seatOption in flight.SeatClassOptions)
            {
                Console.WriteLine($"Class: {seatOption.Class}, Price: {seatOption.Price} EUR");
            }

            Console.WriteLine("Proceeding with the booking process...");
            // todo: proceed with booking
        }
    }

    static public void FilterFlightsByPriceUI()
    {
        FlightsLogic flights = new FlightsLogic();
        string[] filterOptions = new[]
        {
            "Price from low-high",
            "Price from high-low",
            "Price between input range",
            "Filter by destination",
            "Back to Main Menu"
        };

        int selectedIndex = NavigateMenu(filterOptions, "Filter Flights:");

        switch (selectedIndex)
        {
            case 0:
                Console.WriteLine("Enter seat class (Economy, Business, First): ");
                string seatClassAsc = Console.ReadLine();
                foreach (var flight in flights.FilterFlightsByPriceUp(seatClassAsc))
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.SeatClassOptions.FirstOrDefault(option => option.Class == seatClassAsc)?.Price ?? 0} EUR");
                }

                break;
            case 1:
                Console.WriteLine("Enter seat class (Economy, Business, First): ");
                string seatClassDesc = Console.ReadLine();
                foreach (var flight in flights.FilterFlightsByPriceDown(seatClassDesc))
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.SeatClassOptions.FirstOrDefault(option => option.Class == seatClassDesc)?.Price ?? 0} EUR");
                }

                break;
            case 2:
                Console.WriteLine("Enter seat class (Economy, Business, First): ");
                string seatClassRange = Console.ReadLine();
                Console.WriteLine("Enter minimum price: ");
                if (int.TryParse(Console.ReadLine(), out int min))
                {
                    Console.WriteLine("Enter maximum price: ");
                    if (int.TryParse(Console.ReadLine(), out int max))
                    {
                        foreach (var flight in flights.FilterFlightsByPriceRange(seatClassRange, min, max))
                        {
                            Console.WriteLine(
                                $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.SeatClassOptions.FirstOrDefault(option => option.Class == seatClassRange)?.Price ?? 0} EUR");
                        }
                    }
                }

                break;
            case 3:
                Console.WriteLine("Enter destination: ");
                string destination = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(destination))
                {
                    destination = char.ToUpper(destination[0]) + destination.Substring(1);
                    foreach (var flight in flights.FilterFlightsByDestination(destination))
                    {
                        Console.WriteLine(
                            $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.SeatClassOptions.FirstOrDefault()?.Price ?? 0} EUR");
                    }
                }

                break;
            case 4:
                return;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
