public class AirportUI
{
    public static void DisplayAirportHeader()
    {
        Console.WriteLine("\n" + new string('-', 100));
        Console.WriteLine("|{0,-20}|{1,-15}|{2,-20}|{3,-20}|{4,-18}|", 
            " Airport", " Country", " City", " Phone", " Address");
        Console.WriteLine(new string('-', 100));
    }

    public static void DisplayAirportDetails(AirportModel airport)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n=== {airport.Name} ===");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Location: {airport.City}, {airport.Country}");
        Console.WriteLine($"Address: {airport.Address}");
        Console.WriteLine($"Contact: {airport.PhoneNumber}");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nServices & Amenities:");
        Console.ResetColor();

        var airportService = new AirportService(new List<AirportModel> { airport });
        
        Console.WriteLine("\n🚗 Transportation:");
        foreach (var option in airportService.GetAirportTransportationOptions(airport).Split(','))
        {
            Console.WriteLine($"  • {option.Trim()}");
        }

        Console.WriteLine("\n🏨 Nearby Hotels:");
        foreach (var hotel in airportService.GetNearbyHotels(airport).Split(','))
        {
            Console.WriteLine($"  • {hotel.Trim()}");
        }

        Console.WriteLine("\n💼 Additional Services:");
        foreach (var service in airportService.GetAdditionalServices(airport).Split(','))
        {
            Console.WriteLine($"  • {service.Trim()}");
        }

        Console.WriteLine("\nDescription:");
        Console.WriteLine(airportService.GetAirportDescription(airport));
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        
    }
}