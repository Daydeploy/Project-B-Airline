public class SeatLogic
{
    private static List<SeatInfo> GenerateDefaultSeats()
    {
        var seats = new List<SeatInfo>();

        // First Class (Rows 1-3)
        for (int row = 1; row <= 3; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D", "E", "F" })
            {
                seats.Add(new SeatInfo(
                    seatClass: "First",
                    seatNumber: $"{row}{letter}",
                    isAvailable: true,
                    passengerInfo: null
                ));
            }
        }

        // Business Class (Rows 4-8)
        for (int row = 4; row <= 8; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D", "E", "F" })
            {
                seats.Add(new SeatInfo(
                    seatClass: "Business",
                    seatNumber: $"{row}{letter}",
                    isAvailable: true,
                    passengerInfo: null
                ));
            }
        }

        // Economy Class (Rows 9-30)
        for (int row = 9; row <= 30; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D", "E", "F" })
            {
                seats.Add(new SeatInfo(
                    seatClass: "Economy",
                    seatNumber: $"{row}{letter}",
                    isAvailable: true,
                    passengerInfo: null
                ));
            }
        }

        return seats;
    }

    // Helper Method so that we can create all the necessary files first.
    public static void InitializeSeatsForAllFlights()
    {
        foreach (var flight in SeatAccess._flights)
        {
            string filePath = SeatAccess.GetFilePathForFlight(flight.FlightId);

            if (!File.Exists(filePath))
            {
                var seatModel = new SeatModel(
                    flightId: flight.FlightId,
                    seatModels: GenerateDefaultSeats()
                );

                SeatAccess.SaveForFlight(seatModel);
            }
        }
    }
}