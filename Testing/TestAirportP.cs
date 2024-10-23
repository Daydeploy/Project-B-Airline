using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Testing
{
    [TestClass]
    public class TestAirportP
    {
        // Mock classes to simulate behavior of AirportLogic and FlightsLogic
        private class MockAirportLogic
        {
            public List<AirportModel> GetAllAirports()
            {
                return new List<AirportModel>
                {
                    new AirportModel(1, "Netherlands", "RTM", "Rotterdam The Hague Airport", "NL", "010-4463444", "Rotterdam Airportplein 60, 3045 AP Rotterdam"),
                    new AirportModel(2, "Netherlands", "AMS", "Amsterdam Airport Schiphol", "NL", "020-7940800", "Evert van de Beekstraat 202, 1118 CP Schiphol"),
                    new AirportModel(3, "United Kingdom", "LHR", "London Heathrow Airport", "UK", "0844 335 1801", "Longford, Hounslow TW6")
                };
            }
        }

        private class MockFlightsLogic
        {
            public List<FlightModel> AvailableFlights { get; set; }

            public MockFlightsLogic()
            {
                // Ensure we have flights that correspond to the available airports
                AvailableFlights = new List<FlightModel>
                {
                    new FlightModel(1, "RTM", "LHR", "Airline1", "08:00 AM", 100, 10, "F123"),
                    new FlightModel(2, "AMS", "LHR", "Airline2", "09:00 AM", 150, 5, "F456")
                };
            }
        }

        // Test method for ShowDestinations
        [TestMethod]
        public void ShowDestinations_DisplaysAvailableDestinations()
        {
            // Arrange
            var airportLogic = new MockAirportLogic();
            var flightsLogic = new MockFlightsLogic();
            var originalOut = Console.Out;
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            ShowDestinations(airportLogic, flightsLogic);

            // Assert
            var output = sw.ToString();
            StringAssert.Contains(output, "Available Destinations with Flights");
            StringAssert.Contains(output, "Amsterdam Airport Schiphol");
            StringAssert.Contains(output, "Flight from RTM to LHR");
            StringAssert.Contains(output, "Price: 100"); // Adjust based on available flights

            // Reset console output
            Console.SetOut(originalOut);
        }

        private void ShowDestinations(MockAirportLogic airportLogic, MockFlightsLogic flightsLogic)
        {
            var airports = airportLogic.GetAllAirports();
            var availableFlights = flightsLogic.AvailableFlights;

            // Check for valid airports that have available flights
            var validAirports = airports
                .Where(a => availableFlights.Any(f => f.Origin == a.Name))
                .ToList();

            if (validAirports.Count == 0)
            {
                Console.WriteLine("No available flights at the moment.");
                return;
            }

            // Print available destinations
            Console.WriteLine("Available Destinations with Flights:");
            foreach (var airport in validAirports)
            {
                Console.WriteLine($"{airport.Name} - {airport.City}, {airport.Country}");

                // Find flights for this airport
                var flightsForAirport = availableFlights.Where(f => f.Origin == airport.Name).ToList();
                foreach (var flight in flightsForAirport)
                {
                    Console.WriteLine($"Flight from {flight.Origin} to {flight.Destination}");
                    Console.WriteLine($"Price: {flight.Price}");
                }
            }
        }
    }
}
