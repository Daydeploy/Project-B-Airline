using System;
using System.Linq;

static class FlightInformation
{
    public static void EditFlightInformation()
    {
         Console.Clear();
        var flightsLogic = new FlightsLogic();
        var flights = flightsLogic.GetAllFlights();

        // Display flights in table format
        Console.WriteLine("=== Edit Flight Information ===\n");
        FlightDisplay.DisplayFlights(flights);

        // Get flight ID input
        Console.Write("\nEnter Flight ID to edit (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int flightId) || flightId == 0)
        {
            Console.WriteLine("Operation cancelled.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        // Find selected flight
        var selectedFlight = flights.FirstOrDefault(f => f.FlightId == flightId);
        if (selectedFlight == null)
        {
            Console.WriteLine("\nFlight not found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        // Create copy for tracking changes
        var originalFlight = new FlightModel(
            selectedFlight.FlightId,
            selectedFlight.FlightNumber,
            selectedFlight.Origin,
            selectedFlight.OriginCode,
            selectedFlight.Destination,
            selectedFlight.DestinationCode,
            selectedFlight.DepartureTime,
            selectedFlight.ArrivalTime,
            selectedFlight.Distance,
            selectedFlight.PlaneType,
            selectedFlight.DepartureTerminal,
            selectedFlight.ArrivalTerminal,
            selectedFlight.DepartureGate,
            selectedFlight.ArrivalGate,
            selectedFlight.SeatClassOptions,
            selectedFlight.Status,
            selectedFlight.MealService,
            selectedFlight.Taxes
        );

        // Rest of editing logic remains the same
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Editing Flight: {selectedFlight.FlightNumber}\n");
            FlightDisplay.DisplayFlightDetails(selectedFlight);
            Console.WriteLine();

            string[] editOptions = {
                $"Origin: {selectedFlight.Origin}",
                $"Destination: {selectedFlight.Destination}",
                $"Departure Time: {selectedFlight.DepartureTime}",
                $"Arrival Time: {selectedFlight.ArrivalTime}",
                $"Plane Type: {selectedFlight.PlaneType}",
                $"Status: {selectedFlight.Status}",
                "Save Changes",
                "Back to Menu"
            };

            int selectedOption = MenuNavigationService.NavigateMenu(editOptions, "Select field to edit:");
            if (selectedOption == -1 || selectedOption == editOptions.Length - 1) return;

            if (selectedOption == editOptions.Length - 2)
            {
                // Show changes summary
                Console.Clear();
                Console.WriteLine("=== Changes Summary ===\n");
                ShowChanges(originalFlight, selectedFlight);

                Console.Write("\nSave these changes? (Y/N): ");
                if (Console.ReadLine()?.ToUpper() == "Y")
                {
                    try
                    {
                        flightsLogic.UpdateFlight(selectedFlight);
                        Console.WriteLine("Flight information updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating flight: {ex.Message}");
                    }
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }
                continue;
            }

            EditSelectedField(selectedFlight, selectedOption);
        }
    }

    public static void AddNewFlight()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Flight ===\n");
        var flightsLogic = new FlightsLogic();
        var flights = flightsLogic.GetAllFlights();

        var newFlight = CollectFlightDetails();
        if (newFlight == null) return;

        try
        {
            flightsLogic.AddFlight(newFlight);
            Console.WriteLine("\nFlight added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError adding flight: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void DeleteFlightInformation()
    {
        Console.Clear();
        var flightsLogic = new FlightsLogic();
        var flights = flightsLogic.GetAllFlights();
    
        Console.WriteLine("=== Delete Flight Information ===\n");
        FlightDisplay.DisplayFlights(flights);
    
        Console.Write("\nEnter Flight ID to delete (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int flightId) || flightId == 0)
        {
            Console.WriteLine("Operation cancelled.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }
    
        var selectedFlight = flights.FirstOrDefault(f => f.FlightId == flightId);
        if (selectedFlight == null)
        {
            Console.WriteLine("\nFlight not found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }
    
        Console.Clear();
        Console.WriteLine("Selected Flight Details:");
        FlightDisplay.DisplayFlightDetails(selectedFlight);
        
        Console.Write($"\nAre you sure you want to delete flight {selectedFlight.FlightNumber}? (Y/N): ");
        if (Console.ReadLine()?.ToUpper() == "Y")
        {
            try
            {
                flightsLogic.DeleteFlight(selectedFlight.FlightId);
                Console.WriteLine("\nFlight deleted successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError deleting flight: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled.");
        }
    
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ShowChanges(FlightModel original, FlightModel updated)
    {
        if (original.Origin != updated.Origin)
            Console.WriteLine($"Origin: {original.Origin} -> {updated.Origin}");
        if (original.Destination != updated.Destination)
            Console.WriteLine($"Destination: {original.Destination} -> {updated.Destination}");
        if (original.DepartureTime != updated.DepartureTime)
            Console.WriteLine($"Departure Time: {original.DepartureTime} -> {updated.DepartureTime}");
        if (original.ArrivalTime != updated.ArrivalTime)
            Console.WriteLine($"Arrival Time: {original.ArrivalTime} -> {updated.ArrivalTime}");
        if (original.PlaneType != updated.PlaneType)
            Console.WriteLine($"Plane Type: {original.PlaneType} -> {updated.PlaneType}");
        if (original.Status != updated.Status)
            Console.WriteLine($"Status: {original.Status} -> {updated.Status}");
    }

    private static void EditSelectedField(FlightModel flight, int selectedOption)
    {
        Console.Write($"\nEnter new value (ESC to cancel): ");
        string newValue = Console.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(newValue))
        {
            switch (selectedOption)
            {
                case 0: flight.Origin = newValue; break;
                case 1: flight.Destination = newValue; break;
                case 2: flight.DepartureTime = newValue; break;
                case 3: flight.ArrivalTime = newValue; break;
                case 4: flight.PlaneType = newValue; break;
                case 5: flight.Status = newValue; break;
            }
        }
    }

   private static FlightModel CollectFlightDetails()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Flight ===\n");

        Console.Write("Enter flight number: ");
        string flightNumber = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(flightNumber))
        {
            Console.WriteLine("\nError: Flight number is required.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter origin city: ");
        string origin = Console.ReadLine();
        Console.Write("Enter origin airport code: ");
        string originCode = Console.ReadLine()?.ToUpper();
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(originCode))
        {
            Console.WriteLine("\nError: Origin details are required.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter destination city: ");
        string destination = Console.ReadLine();
        Console.Write("Enter destination airport code: ");
        string destinationCode = Console.ReadLine()?.ToUpper();
        if (string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(destinationCode))
        {
            Console.WriteLine("\nError: Destination details are required.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter departure time (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime departureTime))
        {
            Console.WriteLine("\nError: Invalid departure time format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter arrival time (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime arrivalTime))
        {
            Console.WriteLine("\nError: Invalid arrival time format.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        if (departureTime >= arrivalTime)
        {
            Console.WriteLine("\nError: Departure time must be before arrival time.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter distance (km): ");
        if (!int.TryParse(Console.ReadLine(), out int distance))
        {
            Console.WriteLine("\nError: Invalid distance.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter plane type: ");
        string planeType = Console.ReadLine();

        Console.Write("Enter departure terminal: ");
        string departureTerminal = Console.ReadLine();

        Console.Write("Enter arrival terminal: ");
        string arrivalTerminal = Console.ReadLine();

        Console.Write("Enter departure gate: ");
        string departureGate = Console.ReadLine();

        Console.Write("Enter arrival gate: ");
        string arrivalGate = Console.ReadLine();

        var seatClassOptions = new List<SeatClassOption>();
        string[] classes = { "Economy", "Business", "First" };
        foreach (var seatClass in classes)
        {
            Console.Write($"\nEnter price for {seatClass} class (or press Enter to skip): ");
            if (double.TryParse(Console.ReadLine(), out double price))
            {
                Console.WriteLine($"\nEnter seasonal multipliers for {seatClass} class:");
                Console.Write("Summer multiplier (e.g. 1.2 for 20% increase): ");
                double summerMultiplier = 1.0;
                if (!double.TryParse(Console.ReadLine(), out summerMultiplier))
                {
                    summerMultiplier = 1.0;  // Default no change
                }

                Console.Write("Winter multiplier (e.g. 0.8 for 20% decrease): ");
                double winterMultiplier = 1.0;
                if (!double.TryParse(Console.ReadLine(), out winterMultiplier))
                {
                    winterMultiplier = 1.0;  // Default no change
                }

                var seasonalMultiplier = new SeasonalMultiplier(summerMultiplier, winterMultiplier);
                seatClassOptions.Add(new SeatClassOption(seatClass, price) { SeasonalMultiplier = seasonalMultiplier });
            }
        }

        if (!seatClassOptions.Any())
        {
            Console.WriteLine("\nError: At least one seat class is required.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Console.Write("Enter flight status (Scheduled/On Time/Delayed): ");
        string status = Console.ReadLine() ?? "Scheduled";

        var mealService = new List<string>();
        Console.Write("Enter meal service options (comma-separated): ");
        string mealInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(mealInput))
        {
            mealService.AddRange(mealInput.Split(',').Select(m => m.Trim()));
        }

       // Taxes
        Console.Write("Enter country tax rate (% as decimal, e.g. 0.21): ");
        if (!double.TryParse(Console.ReadLine(), out double countryTax))
        {
            countryTax = 0.21; // Default VAT rate
        }

        var airportTaxes = new Dictionary<string, int>();
        airportTaxes[originCode] = 15; // Default departure tax
        airportTaxes[destinationCode] = 15; // Default arrival tax

        var taxes = new Taxes(countryTax, airportTaxes);

        return new FlightModel(
            0,
            flightNumber,
            origin,
            originCode,
            destination,
            destinationCode,
            departureTime.ToString("yyyy-MM-dd HH:mm"),
            arrivalTime.ToString("yyyy-MM-dd HH:mm"),
            distance,
            planeType,
            departureTerminal,
            arrivalTerminal,
            departureGate,
            arrivalGate,
            seatClassOptions,
            status,
            mealService,
            taxes
        );
    }
}