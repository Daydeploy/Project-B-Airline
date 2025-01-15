using System;
using System.Collections.Generic;
using System.Linq;

public class SmallItemsLogic
{
    public List<SmallItemsModel> FetchItemDetails()
    {
        return SmallItemsDataAccess.LoadAll();
    }

    public bool AddItemsToPassenger(List<ShopItemModel> items, int bookingId, int passengerIndex)
    {
        var bookings = BookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);

        if (booking == null)
            return false;

        if (passengerIndex < 0 || passengerIndex >= booking.Passengers.Count)
            return false;

        var passenger = booking.Passengers[passengerIndex];
        passenger.ShopItems.AddRange(items);

        booking.TotalPrice += (int)items.Sum(i => i.Price);

        BookingAccess.WriteAll(bookings);
        return true;
    }
}