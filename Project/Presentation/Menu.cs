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
            Console.Clear();
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
            DisplayMenu(menuItems, selectedIndex);

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

    static private void DisplayMenu(string[] menuItems, int selectedIndex)
    {
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
        Console.WriteLine("Create a new account");
        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        Console.WriteLine("Enter your full name:");
        string fullName = Console.ReadLine();

        if (_userAccountService.CreateAccount(email, password, fullName))
        {
            Console.WriteLine("Account created successfully. Please login.");
        }
        else
        {
            Console.WriteLine("Failed to create account. Email may already be in use.");
        }
    }

    static public void ShowDestinations()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports()
            .Where(a => a.Type == "Public" && a.City != "Rotterdam")
            .ToList(); // Filter out Rotterdam and private airfield 

        Console.WriteLine("\nAvailable Destinations with Flights:");
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
                Console.WriteLine($"{airport.AirportID}. {airport.City}, {airport.Country} - {airport.Name}");
            }
        }

        Console.WriteLine("\nPlease select a destination by entering the Airport ID:");
        int selectedId = int.Parse(Console.ReadLine());
        var selectedAirport = validAirports.FirstOrDefault(a => a.AirportID == selectedId);

        if (selectedAirport != null)
        {
            Console.WriteLine($"\nYou selected: {selectedAirport.City}, {selectedAirport.Country}");
            Console.WriteLine("Do you want to confirm this destination? (y/n)");
            string confirmation = Console.ReadLine().ToLower();
            if (confirmation == "y")
            {
                if (flightDictionary.TryGetValue(selectedAirport.AirportID, out var flight))
                {
                    Console.WriteLine($"Flight from {flight.Origin} to {flight.Destination}");
                    Console.WriteLine($"Departure: {flight.DepartureTime}, Arrival: {flight.ArrivalTime}");
                    Console.WriteLine($"Price: {flight.Price} EUR");
                    Console.WriteLine("Proceeding with the booking process...");
                    // todo: proceed with booking
                }
            }
            else
            {
                Console.WriteLine("Selection canceled.");
            }
        }
        else
        {
            Console.WriteLine("Invalid selection. Returning to the menu.");
        }
    }

    static public void FilterFlightsByPriceUI()
    {
        FlightsLogic flights = new FlightsLogic();

        Console.WriteLine("Filter: ");
        Console.WriteLine("1: Price from low-high");
        Console.WriteLine("2: Price from high-low");
        Console.WriteLine("3: Price between input range");
        Console.WriteLine("4: Filter by destination");
        string input = Console.ReadLine();
        switch (input)
        {
            case "1":
                foreach (var flight in flights.FilterFlightsByPriceUp())
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                }
                break;
            case "2":
                foreach (var flight in flights.FilterFlightsByPriceDown())
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                }
                break;
            case "3":
                Console.WriteLine("Enter minimum price: ");
                int min = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter maximum price: ");
                int max = int.Parse(Console.ReadLine());
                foreach (var flight in flights.FilterFlightsByPriceRange(min, max))
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                }
                break;
            case "4":
                Console.WriteLine("Enter destination: ");
                string destination = Console.ReadLine();
                destination = char.ToUpper(destination[0]) + destination.Substring(1);
                foreach (var flight in flights.FilterFlightsByDestination(destination))
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                }
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }
    }
}
