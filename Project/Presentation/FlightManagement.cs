using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

static class FlightManagement
{
    private static AccountModel account = UserLogin.UserAccountServiceLogic.CurrentAccount;

    public static void ShowAvailableFlights()
    {
        FlightsLogic flights = new FlightsLogic();
        Console.Clear();

        string origin = SelectOrigin(flights);

        if (string.IsNullOrEmpty(origin)) return;

        string destination = SelectDestination(flights, origin);
        if (string.IsNullOrEmpty(destination)) return;

        List<FlightModel> flightsList = flights.GetFlightsByOriginAndDestination(origin, destination).ToList();
        if (flightsList.Count == 0)
        {
            Console.WriteLine($"No flights available from {origin} to {destination}.");
            return;
        }

        DisplayFlightsWithActions(flightsList, UserLogin.UserAccountServiceLogic.IsUserLoggedIn());
        HandleFlightCommands(flightsList, origin, destination, account);
    }

    private static string SelectOrigin(FlightsLogic flights)
    {
        var origins = flights.GetAllOrigins().ToList();
        if (origins.Count == 0)
        {
            Console.WriteLine("No available origins found.");
            return null;
        }

        origins.Add("Back to Main Menu");

        Console.WriteLine("Select your starting location:");
        int originIndex = MenuNavigationService.NavigateMenu(origins.ToArray(), "Available Origins");

        if (originIndex == -1 || originIndex == origins.Count - 1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return null;
        }

        return origins[originIndex];
    }


    private static string SelectDestination(FlightsLogic flights, string origin)
    {
        var destinations = flights.GetDestinationsByOrigin(origin).ToList();
        if (destinations.Count == 0)
        {
            Console.WriteLine($"No destinations available from {origin}.");
            return null;
        }

        destinations.Add("Back to Main Menu");

        Console.WriteLine($"Available destinations from {origin}:");
        int destinationIndex = MenuNavigationService.NavigateMenu(destinations.ToArray(), "Available Destinations");

        if (destinationIndex == -1 || destinationIndex == destinations.Count - 1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return null;
        }

        return destinations[destinationIndex];
    }


    private static void DisplayFlightsWithActions(List<FlightModel> flightsList, bool allowBooking)
    {
        if (account.EmailAddress == "admin")
        {
            allowBooking = false;
        }

        Console.CursorVisible = false;
        FlightDisplay.DisplayFlights(flightsList);

        Console.WriteLine("\nCommands:");
        Console.WriteLine("F - Filter flights");
        if (allowBooking)
            Console.WriteLine("B - Book a flight");
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (account.EmailAddress == "admin")
            {
                Console.WriteLine("B - Book a flight (Admin account cannot book flights)");
            }
            else
            {
                Console.WriteLine("B - Book a flight (Login required)");
            }

            Console.ResetColor();
        }

        Console.WriteLine("ESC - Go back");
    }

    private static void HandleFlightCommands(List<FlightModel> flightsList, string origin, string destination,
        AccountModel account)
    {
        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    Console.WriteLine("\nReturning to the main menu...");
                    return;
                case ConsoleKey.F:
                    FilterFlightsByPriceUI(origin, destination, account);
                    return;
                case ConsoleKey.B:
                    if (UserLogin.UserAccountServiceLogic.IsUserLoggedIn())
                    {
                        Console.Clear();
                        AdvancedBooking(flightsList, account);
                        return;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nYou must be logged in to book a flight.");
                    Console.ResetColor();
                    continue;
            }
        }
    }


    public static void FilterFlightsByPriceUI(string origin, string destination, AccountModel account)
    {
        FlightsLogic flights = new FlightsLogic();
        string[] filterOptions =
        {
            "Price from low-high",
            "Price from high-low",
            "Price between input range",
            "Filter by date range",
            "Back to Main Menu"
        };

        string[] seatClassOptions = { "Economy", "Business", "First" };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(filterOptions, "Filter Flights:");
            if (selectedIndex == -1 || selectedIndex == 4)
            {
                Console.WriteLine("\nReturning to the main menu...");
                return;
            }

            List<FlightModel> filteredFlights = new List<FlightModel>();
            switch (selectedIndex)
            {
                case 0:
                    filteredFlights = FilterByPriceAscending(flights, origin, destination, seatClassOptions);
                    break;
                case 1:
                    filteredFlights = FilterByPriceDescending(flights, origin, destination, seatClassOptions);
                    break;
                case 2:
                    filteredFlights = FilterByPriceRange(flights, origin, destination, seatClassOptions);
                    break;
                case 3:
                    filteredFlights = FilterByDateRange(flights, origin, destination);
                    break;
            }

            if (filteredFlights.Count != 0)
            {
                DisplayFlightsWithActions(filteredFlights, UserLogin.UserAccountServiceLogic.IsUserLoggedIn());
                HandleFlightCommands(filteredFlights, origin, destination, account);
            }
            else
            {
                Console.WriteLine("No flights found matching your criteria.");
                Console.WriteLine("\nPress any key to return to the filter menu...");
                Console.ReadKey();
            }
        }
    }


    private static List<FlightModel> FilterByPriceAscending(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        if (seatClassIndex == -1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return new List<FlightModel>();
        }

        string seatClass = seatClassOptions[seatClassIndex];
        return flights.FilterFlightsByPriceUp(origin, destination, seatClass).ToList();
    }

    private static List<FlightModel> FilterByPriceDescending(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        if (seatClassIndex == -1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return new List<FlightModel>();
        }

        string seatClass = seatClassOptions[seatClassIndex];
        return flights.FilterFlightsByPriceDown(origin, destination, seatClass).ToList();
    }


    private static List<FlightModel> FilterByPriceRange(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        if (seatClassIndex == -1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return new List<FlightModel>();
        }

        string seatClass = seatClassOptions[seatClassIndex];

        Console.WriteLine("Enter minimum price:");
        if (int.TryParse(Console.ReadLine(), out int min))
        {
            Console.WriteLine("Enter maximum price:");
            if (int.TryParse(Console.ReadLine(), out int max))
            {
                return flights.FilterFlightsByPriceRange(origin, destination, seatClass, min, max).ToList();
            }
        }

        Console.WriteLine("Invalid price range entered.");
        return new List<FlightModel>();
    }


    private static List<FlightModel> FilterByDateRange(FlightsLogic flights, string origin, string destination)
    {
        var calendarUI = new CalendarUI();
        var (startDate, endDate) = calendarUI.SelectDateRange();
        return flights.FilterByDateRange(origin, destination, startDate, endDate).ToList();
    }

    public static void BookAFlight(AccountModel account)
    {
        Console.Clear();
        FlightsLogic flightsLogic = new FlightsLogic();

        var origins = flightsLogic.GetAllOrigins();
        if (origins.Count == 0)
        {
            Console.WriteLine("No available origins found.");
            return;
        }

        origins.Add("Back to Main Menu");

        Console.WriteLine("Select your starting location:");
        int originIndex = MenuNavigationService.NavigateMenu(origins.ToArray(), "Available Origins");
        if (originIndex == -1 || originIndex == origins.Count - 1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return;
        }

        string origin = origins[originIndex];

        var destinations = flightsLogic.GetDestinationsByOrigin(origin);
        if (destinations.Count == 0)
        {
            Console.WriteLine($"No destinations available from {origin}.");
            return;
        }

        destinations.Add("Back to Main Menu");

        Console.WriteLine($"Available destinations from {origin}:");
        int destinationIndex = MenuNavigationService.NavigateMenu(destinations.ToArray(), "Available Destinations");
        if (destinationIndex == -1 || destinationIndex == destinations.Count - 1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return;
        }

        string destination = destinations[destinationIndex];

        Console.Clear();
        string[] tripOptions = { "Yes, it's a round-trip booking", "No, one-way trip only", "Back to Main Menu" };
        int tripChoice = MenuNavigationService.NavigateMenu(tripOptions, "Is this a round-trip booking?");
        if (tripChoice == -1 || tripChoice == 2)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return;
        }

        bool isRoundTrip = tripChoice == 0;

        DateTime departureDate = DateTime.Now;
        List<FlightModel> availableFlights;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Please select a departure date:");
            CalendarUI calendar = new CalendarUI(startingDate: departureDate);
            departureDate = calendar.SelectDate();
            if (departureDate == DateTime.MinValue)
            {
                Console.WriteLine("Date selection cancelled.");
                return;
            }

            availableFlights = flightsLogic.FilterFlightsByDate(origin, destination, departureDate);
            if (availableFlights.Count != 0)
            {
                break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nNo flights found from {origin} to {destination} on {departureDate:dd MMM yyyy}.");
            Console.ResetColor();
            Console.ReadKey();
        }

        Console.Clear();
        Console.WriteLine("\nAvailable flights:");
        string[] flightOptions = availableFlights
            .Select((f, index) =>
                $"{f.Origin} → {f.Destination}\n" +
                $"Departure: {DateTime.Parse(f.DepartureTime):HH:mm dd MMM yyyy}  |  Arrival: {DateTime.Parse(f.ArrivalTime):HH:mm dd MMM yyyy}\n" +
                $"Economy: {f.SeatClassOptions[0].Price} EUR  |  Business: {f.SeatClassOptions[1].Price} EUR  |  First: {f.SeatClassOptions[2].Price} EUR\n")
            .ToArray();
        int selectedFlightIndex = MenuNavigationService.NavigateMenu(flightOptions, "Select a departure flight:");
        if (selectedFlightIndex == -1)
        {
            Console.WriteLine("\nReturning to the main menu...");
            return;
        }

        var selectedFlight = availableFlights[selectedFlightIndex];

        DateTime? returnDate = null;
        if (isRoundTrip)
        {
            bool validReturnDate = false;
            while (!validReturnDate)
            {
                Console.Clear();
                Console.WriteLine("Please select a return date:");
                CalendarUI returnCalendar = new CalendarUI(startingDate: returnDate ?? departureDate,
                    highlightDate: departureDate);
                returnDate = returnCalendar.SelectDate();
                if (returnDate == DateTime.MinValue)
                {
                    Console.WriteLine("Return date selection cancelled.");
                    return;
                }

                if (returnDate <= departureDate)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nReturn date must be after the departure date ({departureDate:dd MMM yyyy}).");
                    Console.ResetColor();
                    Console.ReadKey();
                    continue;
                }

                var returnFlights = flightsLogic.FilterFlightsByDate(destination, origin, returnDate.Value);
                if (returnFlights.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"\nNo return flights available from {destination} to {origin} on {returnDate:dd MMM yyyy}.");
                    Console.ResetColor();
                    Console.ReadKey();
                    continue;
                }

                Console.Clear();
                Console.WriteLine("\nAvailable return flights:");
                string[] returnFlightOptions = returnFlights
                    .Select((f, index) =>
                        $"{f.Origin} → {f.Destination}\n" +
                        $"Departure: {DateTime.Parse(f.DepartureTime):HH:mm dd MMM yyyy}  |  Arrival: {DateTime.Parse(f.ArrivalTime):HH:mm dd MMM yyyy}\n" +
                        $"Economy: {f.SeatClassOptions[0].Price} EUR  |  Business: {f.SeatClassOptions[1].Price} EUR  |  First: {f.SeatClassOptions[2].Price} EUR\n")
                    .ToArray();
                int selectedReturnFlightIndex =
                    MenuNavigationService.NavigateMenu(returnFlightOptions, "Select a return flight:");
                if (selectedReturnFlightIndex == -1)
                {
                    Console.WriteLine("\nReturning to the main menu...");
                    return;
                }

                var selectedReturnFlight = returnFlights[selectedReturnFlightIndex];

                HandlePassengerDetailsAndBooking(account, selectedFlight, selectedReturnFlight);
                validReturnDate = true;
            }
        }
        else
        {
            HandlePassengerDetailsAndBooking(account, selectedFlight, null);
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }


    private static void AdvancedBooking(List<FlightModel> flightsList, AccountModel account)
    {
        FlightsLogic flightsLogic = new FlightsLogic();

        Console.WriteLine("Is this a round-trip booking? (y/n):");
        bool isRoundTrip = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;

        Console.WriteLine("Enter the Flight ID for your departure flight:");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        {
            Console.WriteLine("Invalid Flight ID entered.");
            return;
        }

        var selectedFlight = flightsLogic.GetFlightsById(flightId);
        if (selectedFlight == null)
        {
            Console.WriteLine("Flight not found. Please check the Flight ID and try again.");
            return;
        }

        FlightModel returnFlight = null;

        if (isRoundTrip)
        {
            Console.Clear();

            var returnFlights = flightsLogic.GetReturnFlights(selectedFlight);

            if (returnFlights.Count == 0)
            {
                Console.WriteLine("No return flights available. Round-trip booking cannot proceed.");
                return;
            }

            Console.WriteLine("Available Return Flights:");
            DisplayFlightsWithActions(returnFlights, allowBooking: true);

            Console.WriteLine("\nEnter the Flight ID for your return flight:");
            if (!int.TryParse(Console.ReadLine(), out int returnFlightId))
            {
                Console.WriteLine("Invalid Flight ID entered for the return flight.");
                return;
            }

            returnFlight = flightsLogic.GetFlightsById(returnFlightId);
            if (returnFlight == null)
            {
                Console.WriteLine("Return flight not found. Please check the Flight ID and try again.");
                return;
            }
        }

        HandlePassengerDetailsAndBooking(account, selectedFlight, returnFlight);

        Console.WriteLine("\nBooking completed. Press any key to return to the menu...");
        Console.ReadKey();
    }


    private static void HandlePassengerDetailsAndBooking(AccountModel account, FlightModel departureFlight,
        FlightModel? returnFlight)
    {
        var seatSelector = new SeatSelectionUI();
        int availableSeats = seatSelector.GetAvailableSeatsCount(departureFlight.PlaneType, departureFlight.FlightId);

        if (availableSeats == 0)
        {
            Console.WriteLine("\nSorry, this flight is fully booked.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        AccountsAccess.LoadAll();
        int passengerCount;
        bool isValidInput = false;

        do
        {
            Console.WriteLine($"\nAvailable seats: {availableSeats}");
            Console.WriteLine("How many passengers? (1-8):");

            if (!int.TryParse(Console.ReadLine(), out passengerCount) ||
                passengerCount <= 0 ||
                passengerCount > 8 ||
                passengerCount > availableSeats)
            {
                if (passengerCount > availableSeats)
                {
                    Console.WriteLine($"\nSorry, there are only {availableSeats} seats available on this flight.");
                }
                else
                {
                    Console.WriteLine("Invalid number of passengers. Please enter a number between 1 and 8.");
                }

                continue;
            }

            isValidInput = true;
        } while (!isValidInput);

        var passengerDetails = CollectPassengerDetails(departureFlight, passengerCount, seatSelector);

        bool includeInsuranceForDeparture = PromptForInsurance(passengerCount, "departure");

        CompleteBooking(departureFlight.FlightId, passengerDetails, departureFlight, seatSelector,
            includeInsuranceForDeparture);

        if (returnFlight != null)
        {
            foreach (var passenger in passengerDetails)
            {
                Console.WriteLine($"\nSelect a seat for {passenger.Name} on the return flight:");
                string seatNumber = seatSelector.SelectSeat(returnFlight.PlaneType, returnFlight.FlightId);
                seatSelector.SetSeatOccupied(seatNumber);

                if (passenger.HasPet)
                {
                    seatSelector.SetPetSeat(seatNumber);
                }

                passenger.SeatNumber = seatNumber;
            }

            bool includeInsuranceForReturn = PromptForInsurance(passengerCount, "return");

            CompleteBooking(returnFlight.FlightId, passengerDetails, returnFlight, seatSelector,
                includeInsuranceForReturn);
        }
    }


    private static bool PromptForInsurance(int passengerCount, string flightType)
    {
        double insuranceCostPerPassenger = 10.0;
        double totalInsuranceCost = insuranceCostPerPassenger * passengerCount;

        Console.WriteLine("Enter Y to add insurance, or N to skip:");
        string? response = Console.ReadLine()?.Trim().ToLower();
        bool addInsurance = false;
        
        if (response == "y" || response == "yes")
        {
            addInsurance = true;
        }
        
        return addInsurance;
    }

    private static readonly string[] petTypes = { "Dog", "Cat", "Bird", "Rabbit", "Hamster" };

    private static readonly Dictionary<string, double> maxWeights = new Dictionary<string, double>
    {
        { "Dog", 32.0 },
        { "Cat", 15.0 },
        { "Bird", 2.0 },
        { "Rabbit", 8.0 },
        { "Hamster", 1.0 }
    };

    private static List<PassengerModel> CollectPassengerDetails(FlightModel selectedFlight, int passengerCount,
        SeatSelectionUI seatSelector)
    {
        List<PassengerModel> passengerDetails = new List<PassengerModel>();
        string[] yesNoOptions = { "Yes", "No" };

        try
        {
            seatSelector.ClearTemporarySeats();

            for (int i = 0; i < passengerCount; i++)
            {
                Console.Clear();
                Console.WriteLine($"Passenger {i + 1} Details:");
                Console.WriteLine("Enter passenger name:");
                string name = Console.ReadLine();
                while (!AccountsLogic.IsValidName(name))
                {
                    Console.WriteLine(
                        "Name must be between 2 and 20 characters long, start with a capital letter, and cannot contain numbers.");
                    Console.WriteLine("Enter passenger name: ");
                    name = Console.ReadLine();
                }

                int baggageChoice =
                    MenuNavigationService.NavigateMenu(yesNoOptions, "Does this passenger have checked baggage?");
                if (baggageChoice == -1)
                {
                    Console.WriteLine("\nReturning to the main menu...");
                    return null;
                }

                bool hasCheckedBaggage = baggageChoice == 0;

                int numberOfBaggage = 0;
                if (hasCheckedBaggage)
                {
                    bool validInput = false;
                    do
                    {
                        Console.WriteLine($"\nHow many pieces of baggage? (1-3)");
                        Console.WriteLine("First bag is Free. Additional bags cost: 30 EUR each");
                        if (int.TryParse(Console.ReadLine(), out numberOfBaggage) &&
                            numberOfBaggage > 0 &&
                            numberOfBaggage <= 3)
                        {
                            validInput = true;
                            int extraBags = Math.Max(0, numberOfBaggage - 1);
                            int totalCost = extraBags * 30;
                            Console.WriteLine($"\nTotal baggage cost: {totalCost} EUR");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine($"Please enter a number between 1 and {3}");
                        }
                    } while (!validInput);
                }

                int specialLuggageChoice =
                    MenuNavigationService.NavigateMenu(yesNoOptions, "Do you have special luggage?");
                if (specialLuggageChoice == -1)
                {
                    Console.WriteLine("\nReturning to the main menu...");
                    return null;
                }

                bool hasSpecialLuggage = specialLuggageChoice == 0;

                string specialLuggage = "";
                if (hasSpecialLuggage)
                {
                    Console.WriteLine("What special luggage do you have? (e.g. Ski equipment, Musical instrument):");
                    specialLuggage = Console.ReadLine() ?? string.Empty;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nYour {specialLuggage} will be stored securely in the luggage compartment.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }

                int petChoice = MenuNavigationService.NavigateMenu(yesNoOptions, "Does this passenger have any pets?");
                if (petChoice == -1)
                {
                    Console.WriteLine("\nReturning to the main menu...");
                    return null;
                }

                bool hasPet = petChoice == 0;

                List<PetModel> petDetails = null;
                if (hasPet)
                {
                    petDetails = SelectPetDetails(petTypes, maxWeights);
                    if (petDetails.Any(p => p.StorageLocation == "Cabin"))
                    {
                        Console.WriteLine("Note: You will be assigned a suitable seat for traveling with a pet.");
                    }
                }

                var passenger = new PassengerModel(
                    name,
                    null,
                    hasCheckedBaggage,
                    hasPet,
                    petDetails,
                    specialLuggage
                )
                {
                    NumberOfBaggage = numberOfBaggage
                };

                Console.WriteLine("\nSelect a seat for the passenger:");
                string seatNumber = seatSelector.SelectSeat(selectedFlight.PlaneType, selectedFlight.FlightId, passengerDetails);
                if (seatNumber == null)
                {
                    Console.WriteLine("\nSeat selection cancelled. Returning to the main menu...");
                    return null;
                }

                seatSelector.SetSeatOccupied(seatNumber, name);
                if (hasPet && petDetails.Any(p => p.StorageLocation == "Cabin"))
                {
                    seatSelector.SetPetSeat(seatNumber);
                }

                passenger.SeatNumber = seatNumber;
                passengerDetails.Add(passenger);
            }

            seatSelector.CommitTemporarySeats();
            return passengerDetails;
        }
        // try catch exception, aangezien het in een try catch block is zijn we van mening dat de exception kan blijven
        catch (Exception)
        {
            seatSelector.ClearTemporarySeats();
            throw;
        }
    }


    private static List<PetModel> SelectPetDetails(string[] petTypes, Dictionary<string, double> maxWeights)
    {
        List<PetModel> pets = new List<PetModel>();
        int maxPets = 3;
        bool cabinPetSelected = false;

        Console.WriteLine($"\nYou can bring up to {maxPets} pets (one suitable pet in cabin, others in storage)");
        Console.WriteLine("How many pets would you like to bring? (1-3):");

        int petCount;
        while (!int.TryParse(Console.ReadLine(), out petCount) || petCount < 1 || petCount > maxPets)
        {
            Console.WriteLine($"Please enter a number between 1 and {maxPets}:");
        }

        for (int i = 0; i < petCount; i++)
        {
            Console.WriteLine($"\nPet {i + 1} Details:");
            int selectedIndex = MenuNavigationService.NavigateMenu(petTypes, "Select pet type:");
            if (selectedIndex == -1)
            {
                Console.WriteLine("\nReturning to the main menu...");
                return null;
            }

            string selectedPetType = petTypes[selectedIndex];
            double maxWeight = maxWeights[selectedPetType];

            Console.WriteLine($"\nEnter {selectedPetType}'s weight in kg (max {maxWeight}kg):");
            double weight;
            while (!double.TryParse(Console.ReadLine(), out weight) || weight <= 0 || weight > maxWeight)
            {
                Console.WriteLine($"Please enter a valid weight (0-{maxWeight}kg):");
            }

            string storageLocation = "Storage";

            if (!cabinPetSelected && weight <= maxWeight / 2)
            {
                string[] locationOptions = { "Cabin", "Storage" };
                int locationChoice = MenuNavigationService.NavigateMenu(locationOptions,
                    $"This {selectedPetType} is eligible for cabin transport. Select location:");
                if (locationChoice == -1)
                {
                    Console.WriteLine("\nReturning to the main menu...");
                    return null;
                }

                if (locationChoice == 0)
                {
                    storageLocation = "Cabin";
                    cabinPetSelected = true;
                }
            }
            else
            {
                if (cabinPetSelected)
                {
                    Console.WriteLine(
                        "Another pet is already assigned to the cabin. This pet will be placed in storage.");
                }
                else if (weight > maxWeight / 2)
                {
                    Console.WriteLine($"Due to weight restrictions, this {selectedPetType} must be placed in storage.");
                }
            }

            var pet = new PetModel
            {
                Type = selectedPetType,
                Weight = weight,
                StorageLocation = storageLocation
            };

            pets.Add(pet);
            Console.WriteLine($"{selectedPetType} will be transported in: {storageLocation}");
        }

        return pets;
    }

    private static void CompleteBooking(
        int departureFlightId,
        List<PassengerModel> passengerDetails,
        FlightModel departureFlight,
        SeatSelectionUI seatSelector,
        bool includeInsurance,
        FlightModel? returnFlight = null,
        int? returnFlightId = null)
    {
        double totalBaggageCost = 0;
        double totalBasePrice = 0;
        const int BAGGAGE_PRICE = 30;
    
        BookingModel booking = BookingLogic.CreateBooking(userId: UserLogin.UserAccountServiceLogic.CurrentUserId, flightId: departureFlight.FlightId, passengerDetails: passengerDetails, 
            petDetails: new List<PetModel>(), includeInsurance: includeInsurance, isPrivateJet: false
        );
    
        if (booking == null)
        {
            Console.WriteLine("Error: Unable to create booking. Please try again.");
            return;
        }
    
        totalBasePrice = booking.TotalPrice;
    
        foreach (var passenger in booking.Passengers)
        {
            Console.WriteLine($"\nWould you like to purchase items from our shop for {passenger.Name}? (y/n):");
            bool wantsItem = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;
    
            if (wantsItem)
            {
                if (!BookingLogic.SaveBooking(booking))
                {
                    Console.WriteLine("Error: Unable to save booking. Shop purchase cancelled.");
                    continue;
                }

                var shopUI = new ShopUI();
                var purchasedItems = shopUI.DisplaySmallItemsShop(booking.BookingId, booking.Passengers.IndexOf(passenger));
                passenger.ShopItems.AddRange(purchasedItems);
                totalBasePrice += (double)purchasedItems.Sum(item => item.Price);
            }

            // BookingLogic.SaveBooking(booking);

        }
    
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔══════════════════════════════════╗");
        Console.WriteLine("║        BOOKING SUMMARY           ║");
        Console.WriteLine("╚══════════════════════════════════╝\n");
        Console.ResetColor();
    
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Flight Details:");
        Console.ResetColor();
        Console.WriteLine($"From: {departureFlight.Origin} To: {departureFlight.Destination}");
        Console.WriteLine($"Date: {DateTime.Parse(departureFlight.DepartureTime):dd MMM yyyy HH:mm}");
    
        Console.WriteLine("\n" + new string('─', Console.WindowWidth - 1));
    
        foreach (var passenger in booking.Passengers)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nPassenger: {passenger.Name}");
            Console.ResetColor();
            Console.WriteLine($"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
    
            if (passenger.HasCheckedBaggage)
            {
                double passengerBaggageCost = (passenger.NumberOfBaggage - 1) * BAGGAGE_PRICE;
                // totalBaggageCost += passengerBaggageCost;
                Console.WriteLine($"Checked Baggage: {passenger.NumberOfBaggage} piece(s) ({passengerBaggageCost:C})");
            }
    
            if (passenger.ShopItems?.Any() == true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Purchased Items:");
                Console.ResetColor();
                foreach (var item in passenger.ShopItems)
                {
                    Console.WriteLine($"  • {item.Name}: {item.Price:C}");
                }
            }
        }
    
        Console.WriteLine("\n" + new string('─', Console.WindowWidth - 1));
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\nPrice Summary:");
        Console.ResetColor();
        Console.WriteLine($"Base Price: {totalBasePrice:C}");
        if (totalBaggageCost > 0)
            Console.WriteLine($"Total Baggage Cost: {totalBaggageCost:C}");
    
        if (includeInsurance)
        {
            double insuranceCost = 10;
            totalBasePrice += insuranceCost;
            Console.WriteLine($"Cancellation Insurance: {insuranceCost:C}");
        }

    
        int roundedTotalPrice = (int)Math.Round(totalBasePrice);
    
        var accounts = AccountsAccess.LoadAll();
        var currentAccount = accounts.FirstOrDefault(a => a.Id == UserLogin.UserAccountServiceLogic.CurrentUserId);
    
        if (currentAccount?.Miles?.Count > 0)
        {
            var currentMiles = currentAccount.Miles[0];
            Console.WriteLine("\n" + new string('─', Console.WindowWidth - 1));
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nMiles Information:");
            Console.ResetColor();
            Console.WriteLine($"Current Miles Balance: {currentMiles.Points}");
            Console.WriteLine($"Current Tier: {currentMiles.Level}");
    
            if (currentMiles.Points >= 50000)
            {
                int discountPercentage = currentMiles.Level switch
                {
                    "Platinum" => 20,
                    "Gold" => 15,
                    "Silver" => 10,
                    "Bronze" => 5,
                    _ => 0
                };
    
                Console.WriteLine($"\nYou can redeem 50,000 miles for a {discountPercentage}% discount!");
                Console.WriteLine("Would you like to use your miles for a discount? (y/n):");
    
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    var (discountedPrice, discountSuccess) = MilesLogic.BasicPointsRedemption(
                        UserLogin.UserAccountServiceLogic.CurrentUserId,
                        roundedTotalPrice,
                        booking.BookingId
                    );
                    if (discountSuccess)
                    {
                        totalBasePrice = discountedPrice;
                        Console.WriteLine($"\nUpdated Total Price after discount: {discountedPrice:C}");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nYou do not have enough miles for a discount (50,000 required).");
            }
        }
    
        Console.WriteLine("\n" + new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\nFinal Total Price: {totalBasePrice:C}");
        Console.ResetColor();
    
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nWould you like to confirm this booking? (Y/N): ");
        Console.ResetColor();
        
        if (Console.ReadLine()?.ToUpper() != "Y")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBooking cancelled. Returning to menu...");
            Console.ResetColor();
            return;
        }
    
        booking.TotalPrice = (int)Math.Round(totalBasePrice);
        BookingLogic.SaveBooking(booking);
        BookingAccess.WriteAll(BookingAccess.LoadAll());
        
        var (milesEarned, milesEarnedSuccess) = MilesLogic.CalculateMilesFromBooking(UserLogin.UserAccountServiceLogic.CurrentUserId);
        
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n╔══════════════════════════════════╗");
        Console.WriteLine("║        BOOKING COMPLETED         ║");
        Console.WriteLine("╚══════════════════════════════════╝\n");
        Console.ResetColor();
    
        if (milesEarnedSuccess)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Miles Earned: {milesEarned}");
            Console.ResetColor();
        }
    
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nThank you for booking with us!");
        Console.ResetColor();
    }

    public static void ViewBookedFlights(int userId)
    {
        while (true)
        {
            Console.Clear();
            var bookings = BookingAccess.LoadAll().Where(b => b.UserId == userId).ToList();

            if (bookings.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou have no booked flights.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nYou have {bookings.Count} booked flight(s):\n");

            FlightDisplay.DrawBookingsTableHeader();

            foreach (var booking in bookings)
            {
                var flightsLogic = new FlightsLogic();
                FlightModel flight;
                if (booking.FlightId == 0)
                {
                    flight = null;
                }
                else
                {
                    flight = flightsLogic.GetFlightsById(booking.FlightId);
                }

                FlightDisplay.DisplayBookingDetails(booking, flight);
                FlightDisplay.DisplayPassengerDetails(booking.Passengers, booking);
                Console.WriteLine(new string('─', Console.WindowWidth - 1));
            }

            BookingModifications.DisplayBookingLegend();

            Console.Write("\nEnter command: ");
            string command = Console.ReadLine()?.ToLower() ?? "";

            switch (command)
            {
                case "m":
                    BookingModifications.ModifyBookingInteractive(userId);
                    break;
                case "b":
                    return;
                default:
                    Console.WriteLine("Invalid command. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void CheckInForFlight()
    {
        var account = UserLogin.UserAccountServiceLogic.CurrentAccount;

        var bookings = BookingLogic.GetAvailableCheckInBookings(account.Id);

        if (bookings.Count == 0)
        {
            Console.WriteLine("You have no upcoming flights available for check-in.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        var flightsLogic = new FlightsLogic();
        var flightOptions = new List<string>();

        foreach (var booking in bookings)
        {
            var flight = flightsLogic.GetFlightsById(booking.FlightId);
            if (flight != null)
            {
                string flightInfo = $"Flight {flight.FlightNumber}: {flight.Origin} → {flight.Destination}\n" +
                                    $"Departure: {DateTime.Parse(flight.DepartureTime):dd MMM yyyy HH:mm}\n" +
                                    $"Booking ID: {booking.BookingId}\n";
                flightOptions.Add(flightInfo);
            }
        }

        flightOptions.Add("Back to Main Menu");

        while (true)
        {
            Console.Clear();
            int selectedIndex =
                MenuNavigationService.NavigateMenu(flightOptions.ToArray(), "Select Flight for Check-in");

            if (selectedIndex == flightOptions.Count - 1 || selectedIndex == -1)
            {
                return;
            }

            var selectedBooking = bookings[selectedIndex];
            var selectedFlight = flightsLogic.GetFlightsById(selectedBooking.FlightId);

            if (selectedFlight != null)
            {
                Console.Clear();
                Console.WriteLine($"Checking in for flight {selectedFlight.FlightNumber}");
                Console.WriteLine($"From: {selectedFlight.Origin} To: {selectedFlight.Destination}");
                Console.WriteLine($"Departure: {DateTime.Parse(selectedFlight.DepartureTime):dd MMM yyyy HH:mm}");
                Console.WriteLine("\nPassengers:");

                foreach (var passenger in selectedBooking.Passengers)
                {
                    Console.WriteLine($"- {passenger.Name} (Seat: {passenger.SeatNumber})");
                }

                Console.WriteLine("\nConfirm check-in? (Y/N):");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    if (BookingLogic.CheckInBooking(selectedBooking.BookingId))
                    {
                        UserLogin.UserAccountServiceLogic.CheckIn(selectedFlight.FlightId);
                        Console.WriteLine("\nCheck-in successful!");
                        Console.WriteLine("Your boarding passes have been generated.");
                    }
                    else
                    {
                        Console.WriteLine("\nCheck-in failed. Please try again or contact support.");
                    }

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }
        }
    }

    public static void BookPrivateJet(int userId)
    {
        var user = userId;
        string[] jetOptions =
        {
            "Bombardier Learjet 75 (6 seats)",
            "Bombardier Global 8280 (8 seats)",
            "Back to Main Menu"
        };

        Console.Clear();
        int jetChoice = MenuNavigationService.NavigateMenu(jetOptions, "Select Private Jet");

        if (jetChoice == 2 || jetChoice == -1)
        {
            return;
        }

        string jetType;
        int maxPassengers;

        if (jetChoice == 0)
        {
            jetType = "Bombardier Learjet 75";
            maxPassengers = 6;
        }
        else
        {
            jetType = "Bombardier Global 8280";
            maxPassengers = 8;
        }

        Console.Clear();
        Console.WriteLine($"Selected jet: {jetType}");
        Console.WriteLine($"\nHow many passengers? (1-{maxPassengers}):");

        int passengerCount;
        while (!int.TryParse(Console.ReadLine(), out passengerCount) ||
               passengerCount < 1 ||
               passengerCount > maxPassengers)
        {
            Console.WriteLine($"Please enter a valid number between 1 and {maxPassengers}:");
        }

        // Collect passenger details
        List<PassengerModel> passengers = new List<PassengerModel>();
        for (int i = 0; i < passengerCount; i++)
        {
            Console.Clear();
            Console.WriteLine($"Passenger {i + 1} Details:");
            Console.WriteLine("Enter passenger name:");
            string name = Console.ReadLine();
            while (!AccountsLogic.IsValidName(name))
            {
                Console.WriteLine(
                    "Name must be between 2 and 20 characters long, start with a capital letter, and cannot contain numbers.");
                Console.WriteLine("Enter passenger name: ");
                name = Console.ReadLine();
            }

            var passenger = new PassengerModel(name, $"PJ{i + 1}", false);
            passengers.Add(passenger);
        }

        // Create booking
        BookingModel booking = BookingLogic.CreateBooking(
            user,
            0, // special flightID for private jets
            passengers,
            new List<PetModel>(), // no pets
            true,
            true,
            jetType
        );

        if (booking == null)
        {
            Console.WriteLine("\nError: Unable to create private jet booking. Please try again.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        // Display booking confirmation
        Console.Clear();
        Console.WriteLine("\nPrivate jet booking completed successfully!");
        Console.WriteLine($"Booking ID: {booking.BookingId}");
        Console.WriteLine($"Aircraft: {booking.PlaneType}");
        Console.WriteLine($"Total Price: {booking.TotalPrice:C}");
        Console.WriteLine("\nPassenger Details:");
        foreach (var passenger in passengers)
        {
            Console.WriteLine($"- {passenger.Name} (Seat: {passenger.SeatNumber})");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}