static class Menu
{
    static private UserAccountService _userAccountService = new UserAccountService();

    static public void Start()
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create Account");
            Console.WriteLine("3. Show available Flights");
            Console.WriteLine("4. View Available Flights by Destination");
            Console.WriteLine("5. Exit");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    UserLogin.Start();
                    break;
                case "2":
                    CreateAccount();
                    break;
                case "3":
                    UserLogin.ShowAvailableFlights();
                    break;
                case "4":
                    ShowDestinations();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
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

    private static void ShowDestinations()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports()
            .Where(a => a.Type == "Public" && a.City != "Rotterdam")
            .ToList(); // Filter out Rotterdam and private airfield 

        Console.WriteLine("\nAvailable Destinations with Flights:");
        var validAirports = new List<AirportModel>(); // List of destinations that have flights available
        var flightDictionary = new Dictionary<int, FlightModel>(); // Store flights by AirportID

        // Assuming FlightsLogic.AvailableFlights is a list of FlightModel that contains all available flights
        var availableFlights = FlightsLogic.AvailableFlights;

        foreach (var airport in airports)
        {
            // Find a flight that matches the current airport's city
            var flightAvailable = availableFlights.FirstOrDefault(f => f.Destination == airport.City);

            // If a flight exists and matches the destination, show it to the user
            if (flightAvailable != null)
            {
                validAirports.Add(airport); // Add to valid destinations
                flightDictionary[airport.AirportID] = flightAvailable; // Store the flight by AirportID
                Console.WriteLine($"{airport.AirportID}. {airport.City}, {airport.Country} - {airport.Name}");
            }
        }

        // Allow the user to select a destination
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
                // Retrieve the flight from the dictionary based on the selected AirportID
                if (flightDictionary.TryGetValue(selectedAirport.AirportID, out var flight))
                {
                    Console.WriteLine($"Flight from {flight.Origin} to {flight.Destination}");
                    Console.WriteLine($"Departure: {flight.DepartureTime}, Arrival: {flight.ArrivalTime}");
                    Console.WriteLine($"Price: {flight.Price} EUR");
                    Console.WriteLine("Proceeding with the booking process...");
                    // todo: zet booking 
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
                // turn first letter to uppercase
                destination = char.ToUpper(destination[0]) + destination.Substring(1);
                foreach (var flight in flights.FilterFlightsByDestination(destination))
                {
                    Console.WriteLine(
                        $"{flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
                }

                ;
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }
    }
}