public class EntertainmentLogic
{
    public (bool success, string errorMessage) AddEntertainmentToBooking(int bookingId, int entertainmentId)
    {
        var bookings = BookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return (false, "Booking not found");

        var entertainmentOption = EntertainmentDataAccess.GetEntertainment(entertainmentId);
        if (entertainmentOption == null) return (false, "Entertainment option not found");

        var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
        if (flight == null) return (false, "Flight not found");

        var seatSelector = new SeatSelectionLogic();
        var seatClass = seatSelector.GetSeatClass(booking.Passengers[0].SeatNumber, flight.PlaneType);

        if (!entertainmentOption.AvailableIn.Contains(seatClass, StringComparer.OrdinalIgnoreCase))
            return (false, $"Entertainment option not available for {seatClass} class");

        booking.Entertainment ??= new List<EntertainmentModel>();

        booking.Entertainment.Add(entertainmentOption);
        booking.TotalPrice += (int)entertainmentOption.Cost;

        BookingAccess.WriteAll(bookings);
        return (true, string.Empty);
    }
}