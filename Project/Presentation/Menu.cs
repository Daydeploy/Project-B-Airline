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
            Console.WriteLine("4. View Destination Information");
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
                    ShowAvailableFlights();
                    return;
                case "4":
                    ViewDestinationInformation();
                    break;
                case "5":
                    break;
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

    private static void ViewDestinationInformation()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        foreach (var airport in airports)
        {
            if (airport.Type == "Public")
            {
                Console.WriteLine($"{airport.City}, {airport.Country}");
                Console.WriteLine($"    - {airport.Name}");
            }
        }
    }

    static private void ShowAvailableFlights(){
        FlightsLogic flights = new FlightsLogic();
        Console.WriteLine("Available Flights:");
        foreach (var flight in flights.GetAllFlights()){
            // Display all flight information
            Console.WriteLine(flight.Origin + " to " + flight.Destination + " at " + flight.DepartureTime + " for " + flight.Price + " EUR");
        }
        Console.WriteLine("Do you want to filter the flights (y/n)");
        string input = Console.ReadLine();
        if (input == "y"){
            FilterFlightsByPriceUI();
        }
    }

    static private void FilterFlightsByPriceUI(){
        FlightsLogic flights = new FlightsLogic();

        Console.WriteLine("Filter: ");
        Console.WriteLine("1: low-high");
        Console.WriteLine("2: high-low");
        Console.WriteLine("3: input range");
        string input = Console.ReadLine();
        switch (input){
            case "1":
                foreach (var flight in flights.FilterFlightsByPriceUp()){
                    Console.WriteLine(flight.Origin + " to " + flight.Destination + " at " + flight.DepartureTime + " for " + flight.Price + " EUR");
                }
                break;
            case "2":
                foreach (var flight in flights.FilterFlightsByPriceDown()){
                    Console.WriteLine(flight.Origin + " to " + flight.Destination + " at " + flight.DepartureTime + " for " + flight.Price + " EUR");
                }
                break;
            case "3":
                Console.WriteLine("Enter minimum price: ");
                int min = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter maximum price: ");
                int max = int.Parse(Console.ReadLine());
                foreach (var flight in flights.FilterFlightsByPriceRange(min, max)){
                    Console.WriteLine(flight.Origin + " to " + flight.Destination + " at " + flight.DepartureTime + " for " + flight.Price + " EUR");
                };
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }

    }
}