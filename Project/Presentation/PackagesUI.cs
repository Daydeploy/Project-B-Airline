using System;
using System.Collections.Generic;

public class PackagesUI
{
    private ComfortPackageService _comfortPackageService = new ComfortPackageService();

    public void ShowPackages()
    {
        List<BookingModel> bookedFlights = BookingDataAccess.LoadAllBookings();

        if (bookedFlights.Count == 0)
        {
            Console.WriteLine("You have no booked flights.");
            return;
        }

        Console.WriteLine("Your Booked Flights:");
        foreach (var booking in bookedFlights)
        {
            Console.WriteLine($"Booking ID: {booking.BookingId}, Flight ID: {booking.FlightId}");
        }

        Console.WriteLine("Enter the Booking ID to purchase a comfort package:");
        int bookingId = int.Parse(Console.ReadLine() ?? "0");

        ComfortPackageUI comfortPackageUI = new ComfortPackageUI();
        comfortPackageUI.RenderComfortPackageOption();

        Console.WriteLine("Please enter the package ID to add it to your booking:");
        int packageId = int.Parse(Console.ReadLine() ?? "0");

        string flightClass = "Economy";
        if (_comfortPackageService.ValidatePackageAvailability(flightClass))
        {
            _comfortPackageService.AddPackageToBooking(bookingId, packageId);
            Console.WriteLine("Comfort package added to your booking.");
        }
        else
        {
            comfortPackageUI.ShowValidationErrors("The comfort package is not available for your selected flight class.");
        }
    }
} 