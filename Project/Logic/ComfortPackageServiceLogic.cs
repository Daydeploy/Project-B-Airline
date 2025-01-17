public class ComfortPackageServiceLogic
{
    public (bool success, string error) AddPackageToBooking(int bookingId, int packageId)
    {
        IBookingAccess _bookingAccess = new BookingAccess();
        var bookings = _bookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null) return (false, "Booking not found");

        var package = ComfortPackageDataAccess.GetComfortPackage(packageId);
        if (package == null) return (false, "Package not found");

        var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
        if (flight == null) return (false, "Flight not found");

        var seatSelector = new SeatSelectionUI();
        var seatClass = seatSelector.GetSeatClass(booking.Passengers[0].SeatNumber, flight.PlaneType);

        if (!package.AvailableIn.Contains(seatClass, StringComparer.OrdinalIgnoreCase))
            return (false, $"Package not available for {seatClass} class");

        booking.ComfortPackages ??= new List<ComfortPackageModel>();

        booking.ComfortPackages.Add(package);
        booking.TotalPrice += (int)package.Cost;

        _bookingAccess.WriteAll(bookings);
        return (true, string.Empty);
    }
}