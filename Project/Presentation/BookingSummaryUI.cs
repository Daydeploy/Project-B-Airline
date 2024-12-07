using System;

public class BookingSummaryUI
{
    public void RenderUpdatedSummary(int bookingId)
    {
        var comfortPackage = ComfortPackageDataAccess.GetBookingComfortDetails(bookingId);
        if (comfortPackage != null)
        {
            Console.WriteLine("Booking Summary:");
            Console.WriteLine($"Comfort Package: {comfortPackage.Name}");
            Console.WriteLine($"Contents: {string.Join(", ", comfortPackage.Contents)}");
            Console.WriteLine($"Cost: {comfortPackage.Cost:C}");
        }
        else
        {
            Console.WriteLine("No comfort package selected.");
        }
    }
}