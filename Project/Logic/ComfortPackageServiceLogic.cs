public class ComfortPackageServiceLogic
{
    public bool ValidatePackageAvailability(string flightClass)
    {
        var package = ComfortPackageDataAccess.GetComfortPackage(1);
        return package != null && package.AvailableIn.Contains(flightClass);
    }

    public void AddPackageToBooking(int bookingId, int packageId)
    {
        try
        {
            var bookings = BookingAccess.LoadAll();
            var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId) 
                ?? throw new Exception("Booking not found");

            var package = ComfortPackageDataAccess.GetComfortPackage(packageId) 
                ?? throw new Exception("Package not found");

            var flight = new FlightsLogic().GetFlightsById(booking.FlightId) 
                ?? throw new Exception("Flight not found");

            var seatSelector = new SeatSelectionUI();
            string seatClass = seatSelector.GetSeatClass(booking.Passengers[0].SeatNumber, flight.PlaneType);

            if (!package.AvailableIn.Contains(seatClass, StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception($"Package not available for {seatClass} class");
            }

            // Initialize ComfortPackages if null
            booking.ComfortPackages ??= new List<ComfortPackageModel>();
            
            // Add package and update price
            booking.ComfortPackages.Add(package);
            booking.TotalPrice += (int)package.Cost;

            BookingAccess.WriteAll(bookings);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add comfort package: {ex.Message}");
        }
    }
}