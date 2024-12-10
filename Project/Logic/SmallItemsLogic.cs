using System;
using System.Collections.Generic;
using System.Linq;

public class SmallItemsLogic
{
    public List<SmallItemsModel> FetchItemDetails()
    {
        return SmallItemsDataAccess.LoadAll();
    }

    public void AddItemsToPassenger(List<ShopItemModel> items, int bookingId, int passengerIndex)
    {
        var bookings = BookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        
        if (booking == null)
            throw new Exception("Booking not found");

        if (passengerIndex < 0 || passengerIndex >= booking.Passengers.Count)
            throw new Exception("Invalid passenger index");

        var passenger = booking.Passengers[passengerIndex];
        passenger.ShopItems.AddRange(items);
        
        // Update total price for booking
        booking.TotalPrice += (int)items.Sum(i => i.Price);
        
        BookingAccess.WriteAll(bookings);
    }
}