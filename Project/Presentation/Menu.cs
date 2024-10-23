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
            "Exit"
        };

        int selectedIndex = 0;
        bool exit = false;

        while (!exit)
        {
            DisplayMenu(menuItems, selectedIndex, "Main Menu");
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
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : menuItems.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < menuItems.Length - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    HandleSelection(menuItems[selectedIndex], ref exit);
                    break;
            }
        }
    }

    // Reusable DisplayMenu method with a title
    static public void DisplayMenu(string[] menuItems, int selectedIndex, string title = "")
    {
        Console.Clear();

        // Display the title, if provided
        if (!string.IsNullOrEmpty(title))
        {
            Console.WriteLine(title);
            Console.WriteLine(new string('-', title.Length)); // Underline the title
        }

        // Display the menu items
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(menuItems[i]);
            Console.ResetColor(); // Reset the colors
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
        // Inform user about F2 toggle
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.\n");
        Console.WriteLine("Create a new account");
        Console.WriteLine("Enter your full name:");
        string fullName = Console.ReadLine();
        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();
        string password = "";
        string confirmPassword = "";
        bool showPassword = false;
        ConsoleKeyInfo key;

        // Password input
        Console.Write("Enter your password: ");
        password = UserLogin.ReadPassword(ref showPassword);

        // Confirm password input
        Console.Write("Confirm your password: ");
        confirmPassword = UserLogin.ReadPassword(ref showPassword);
        
        // Check if passwords match
        if (password == confirmPassword)
        {
            Console.WriteLine("\nPasswords match!");
            
            if (_userAccountService.CreateAccount(email, password, fullName))
            {
                Console.WriteLine("Account created successfully. Please login.");
            }
            else
            {
                Console.WriteLine("Failed to create account. Email may already be in use.");
            }
        }
        else
        {
            Console.WriteLine("\nPasswords do not match. Please try again.");
        }
        
    }

    static public void ShowDestinations()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports()
            .Where(a => a.Type == "Public" && a.City != "Rotterdam")
            .ToList(); // Filter out Rotterdam and private airfield 

        var validAirports = new List<AirportModel>(); // List of destinations that have flights available
        var flightDictionary = new Dictionary<int, FlightModel>(); // Store flights by AirportID

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

        int selectedIndex = 0;
        bool destinationSelected = false;

        string[] destinationMenuItems = validAirports
            .Select(a => $"{a.AirportID}. {a.City}, {a.Country} - {a.Name}")
            .ToArray();

        while (!destinationSelected)
        {
            DisplayMenu(destinationMenuItems, selectedIndex, "Available Destinations with Flights");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : validAirports.Count - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < validAirports.Count - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    var selectedAirport = validAirports[selectedIndex];
                    Console.Clear();
                    Console.WriteLine($"\nYou selected: {selectedAirport.City}, {selectedAirport.Country}");

                    if (flightDictionary.TryGetValue(selectedAirport.AirportID, out var flight))
                    {
                        Console.WriteLine($"Flight from {flight.Origin} to {flight.Destination}");

                        // If they are strings, parse them into DateTime
                        DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
                        DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);

                        // Now format them as needed
                        string formattedDepartureTime = departureDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                        string formattedArrivalTime = arrivalDateTime.ToString("dddd, dd MMMM yyyy HH:mm");

                        Console.WriteLine($"Departure: {formattedDepartureTime}");
                        Console.WriteLine($"Arrival: {formattedArrivalTime}");
                        Console.WriteLine($"Price: {flight.Price} EUR");
                        Console.WriteLine("Proceeding with the booking process...");
                        // todo: proceed with booking
                    }

                    destinationSelected = true;
                    break;
                case ConsoleKey.Escape:
                    destinationSelected = true; // Exit without selecting
                    break;
            }
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

        int selectedIndex = 0; // Start at the first option

        while (true) // Loop until a valid selection is made
        {
            Console.Clear(); // Clear the console for a fresh display
            Console.WriteLine("Filter Flights: ");
            DisplayMenu(filterOptions, selectedIndex);

            var key = Console.ReadKey(intercept: true).Key; // Read key input without displaying it

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex =
                        (selectedIndex == 0) ? filterOptions.Length - 1 : selectedIndex - 1; // Loop to the end
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex =
                        (selectedIndex == filterOptions.Length - 1) ? 0 : selectedIndex + 1; // Loop to the beginning
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            // Price from low to high
                            foreach (var flight in flights.FilterFlightsByPriceUp())
                            {
                                Console.WriteLine(
                                    $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                            }

                            break;
                        case 1:
                            // Price from high to low
                            foreach (var flight in flights.FilterFlightsByPriceDown())
                            {
                                Console.WriteLine(
                                    $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                            }

                            break;
                        case 2:
                            // Price between input range
                            Console.WriteLine("Enter minimum price: ");
                            if (int.TryParse(Console.ReadLine(), out int min))
                            {
                                Console.WriteLine("Enter maximum price: ");
                                if (int.TryParse(Console.ReadLine(), out int max))
                                {
                                    foreach (var flight in flights.FilterFlightsByPriceRange(min, max))
                                    {
                                        Console.WriteLine(
                                            $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                                    }
                                }
                            }

                            break;
                        case 3:
                            // Filter by destination
                            Console.WriteLine("Enter destination: ");
                            string destination = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(destination))
                            {
                                destination = char.ToUpper(destination[0]) + destination.Substring(1);
                                foreach (var flight in flights.FilterFlightsByDestination(destination))
                                {
                                    Console.WriteLine(
                                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                                }
                            }

                            break;
                        case 4:
                            // Back to main menu
                            return; // Exit the loop and return to the main menu
                    }

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    break; // Ignore any other keys
            }
        }
    }
}