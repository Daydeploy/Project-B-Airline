using System;
using System.Collections.Generic;

static class AirportInformation
{
    // Allows the user to view information for various airports
    public static void ViewAirportInformation()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        int currentIndex = 0;
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

        // Displays the current airport details
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

    // Displays a list of all available destinations
    public static void BrowseDestinations()
    {
        FlightsLogic flightsLogic = new FlightsLogic();
        var destinations = flightsLogic.GetAllDestinations();

        Console.WriteLine("\nAvailable Destinations:");
        foreach (var destination in destinations)
        {
            Console.WriteLine(destination);
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    // Searches for flights by a specified destination
    public static void SearchFlightsByDestination()
    {
        Console.WriteLine("\nEnter the destination you want to search for:");
        string? destination = Console.ReadLine() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(destination))
        {
            FlightsLogic flightsLogic = new FlightsLogic();
            var flights = flightsLogic.SearchFlightsByDestination(destination);

            if (flights.Count == 0)
            {
                Console.WriteLine($"No flights found for destination: {destination}");
            }
            else
            {
                Console.WriteLine($"\nFlights to {destination}:");
                foreach (var flight in flights)
                {
                    Console.WriteLine(
                        $"Flight ID: {flight.FlightId}, From: {flight.Origin}, Departure: {flight.DepartureTime}");
                    Console.WriteLine("Prices:");
                    foreach (var seatOption in flight.SeatClassOptions)
                    {
                        Console.WriteLine($"Class: {seatOption.Class}, Price: {seatOption.Price} EUR");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid destination.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}