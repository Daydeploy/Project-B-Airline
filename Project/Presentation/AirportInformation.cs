using System;
using System.Collections.Generic;

static class AirportInformation
{
    public static void ViewAirportInformation()
    {
        while (true)
        {
            Console.Clear();
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
