public class SmallItemsLogic
{
    private readonly IBookingAccess _bookingAccess = new BookingAccess();
    public List<SmallItemsModel> FetchItemDetails()
    {
        return SmallItemsDataAccess.LoadAll();
    }

    public bool AddItemsToPassenger(List<ShopItemModel> items, int bookingId, int passengerIndex)
    {
        var bookings = _bookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

        if (booking == null)
            return false;

        if (passengerIndex < 0 || passengerIndex >= booking.Passengers.Count)
            return false;

        var passenger = booking.Passengers[passengerIndex];
        passenger.ShopItems.AddRange(items);

        booking.TotalPrice += (int)items.Sum(i => i.Price);

        _bookingAccess.WriteAll(bookings);
        return true;
    }
}