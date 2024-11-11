public class SeatLogic
{
    private static List<SeatInfo> GenerateDefaultSeats()
    {
        var seats = new List<SeatInfo>();

        // First Class (Rows 1-5)
        for (int row = 1; row <= 5; row++)
        {
            foreach (var letter in new[] { "A", "B" })
            {
                seats.Add(new SeatInfo(
                    seatClass: "First",
                    seatNumber: $"{row}{letter}",
                    isAvailable: true,
                    passengerInfo: null
                ));
            }
        }

        // Business Class (Rows 6-12)
        for (int row = 6; row <= 12; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D" })
            {
                seats.Add(new SeatInfo(
                    seatClass: "Business",
                    seatNumber: $"{row}{letter}",
                    isAvailable: true,
                    passengerInfo: null
                ));
            }
        }

        // Economy Class (Rows 13-25)
        for (int row = 13; row <= 25; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D" })
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