using System;
using System.Collections.Generic;
using System.Linq;

static class Menu
{
    static private UserAccountService _userAccountService = new UserAccountService();
    private static MenuNavigationService _menuNavigationService = new MenuNavigationService();

    static public void Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false; // Hide the cursor
        List<string> menuItems = new List<string>
        {
            "Login",
            "Create Account",
            "Show Available Flights",
            "Exit"
        };

        while (true)
        {
            _menuNavigationService.DisplayMenu(menuItems);
            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                int selection = _menuNavigationService.HandleMenuSelection(userInput, menuItems);
                if (selection == -1) continue; // Invalid selection, prompt again
                if (selection == 0) // Go back
                {
                    _menuNavigationService.NavigateBack();
                    continue;
                }
                // Handle valid selection
                // Call the corresponding method based on selection
                // Example: HandleSelection(menuItems[selection - 1]);
                _menuNavigationService.ClearScreen();
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
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
            DisplayFlightDetails(flight);
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
            "Filter by date range",
            "Filter by destination and date range",
            "Back to Main Menu"
        };

        int selectedIndex = NavigateMenu(filterOptions, "Filter Flights:");

        string[] seatClassOptions = { "Economy", "Business", "First" };

        switch (selectedIndex)
        {
            case 0:
                int seatClassAscIndex = NavigateMenu(seatClassOptions, "Seat Class");
                string seatClassAsc = seatClassOptions[seatClassAscIndex];
                UserLogin.DisplayFlights(flights.FilterFlightsByPriceUp(seatClassAsc));
                break;

            case 1:
                int seatClassDescIndex = NavigateMenu(seatClassOptions, "Seat Class");
                string seatClassDesc = seatClassOptions[seatClassDescIndex];
                UserLogin.DisplayFlights(flights.FilterFlightsByPriceDown(seatClassDesc));
                break;

            case 2:
                int seatClassRangeIndex = NavigateMenu(seatClassOptions, "Seat Class");
                string seatClassRange = seatClassOptions[seatClassRangeIndex];
                Console.WriteLine("Enter minimum price: ");
                if (int.TryParse(Console.ReadLine(), out int min))
                {
                    Console.WriteLine("Enter maximum price: ");
                    if (int.TryParse(Console.ReadLine(), out int max))
                    {
                        UserLogin.DisplayFlights(flights.FilterFlightsByPriceRange(seatClassRange, min, max));
                    }
                }

                break;

            case 3:
                var destinations = flights.GetAllDestinations().ToArray();
                int destinationIndex = NavigateMenu(destinations, "Select Destination");
                string selectedDestination = destinations[destinationIndex];
                UserLogin.DisplayFlights(flights.FilterFlightsByDestination(selectedDestination));
                break;
                

             case 4:
                var calendarUI = new CalendarUI();
                var (startDate, endDate) = calendarUI.SelectDateRange();
                UserLogin.DisplayFlights(flights.FilterByDateRange(startDate, endDate));
                break;

            case 5:
                var destinations2 = flights.GetAllDestinations().ToArray();
                int destinationIndex2 = NavigateMenu(destinations2, "Select Destination");
                string selectedDestination2 = destinations2[destinationIndex2];
                
                var calendarUI2 = new CalendarUI();
                var (startDate2, endDate2) = calendarUI2.SelectDateRange();
                var filteredFlights = flights.FilterByDateRange(startDate2, endDate2)
                    .Where(f => f.Destination == selectedDestination2)
                    .ToList();
                UserLogin.DisplayFlights(filteredFlights);
                break;
            case 6:
                return;
        }
    }

        static private void DisplayFlightDetails(FlightModel flight)
    {
        DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
        DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);

        Console.WriteLine($"Flight ID : {flight.FlightId}");
        Console.WriteLine($"Route     : {flight.Origin} ➔ {flight.Destination}");
        Console.WriteLine($"Departure : {departureDateTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Arrival   : {arrivalDateTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine("\nPrices:");

        for (int i = 0; i < flight.SeatClassOptions.Count; i++)
        {
            var seatOption = flight.SeatClassOptions[i];
            string prefix = (i == flight.SeatClassOptions.Count - 1) ? "└─" : "├─";
            Console.WriteLine($"  {prefix} Class: {seatOption.Class,-9} | Price: {seatOption.Price} EUR");
        }

        Console.WriteLine(new string('-', 40));
    }

    static private string AirlineLogo()
    {
        return @"
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
";
    }
}

// test