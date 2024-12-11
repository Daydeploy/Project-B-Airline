public class EntertainmentLogic
{
    public void AddEntertainmentToBooking(int bookingId, int entertainmentId)
    {
        try
        {
            var bookings = BookingAccess.LoadAll();
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId) 
                ?? throw new Exception("Booking not found");

            var entertainmentOption = EntertainmentDataAccess.GetEntertainment(entertainmentId) 
                ?? throw new Exception("Entertainment option not found");

            var flight = new FlightsLogic().GetFlightsById(booking.FlightId) 
                ?? throw new Exception("Flight not found");

            var seatSelector = new SeatSelectionUI();
            string seatClass = seatSelector.GetSeatClass(booking.Passengers[0].SeatNumber, flight.PlaneType);

            if (!entertainmentOption.AvailableIn.Contains(seatClass, StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception($"Entertainment option not available for {seatClass} class");
            }

            // Initialize entertainmentOption if null
            booking.Entertainment ??= new List<EntertainmentModel>();
            
            booking.Entertainment.Add(entertainmentOption);
            booking.TotalPrice += (int)entertainmentOption.Cost;

            BookingAccess.WriteAll(bookings);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add entertainment: {ex.Message}");
        }
    }
}