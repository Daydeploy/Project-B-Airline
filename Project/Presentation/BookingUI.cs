using System;

public class BookingUI
{
    private ComfortPackageUI _comfortPackageUI = new ComfortPackageUI();
    private ComfortPackageService _comfortPackageService = new ComfortPackageService();

    public void StartBookingProcess()
    {

        Console.WriteLine("Would you like to add a comfort package to your booking? (yes/no)");
        string response = Console.ReadLine()?.ToLower();

        if (response == "yes")
        {
            _comfortPackageUI.RenderComfortPackageOption();
            Console.WriteLine("Please enter the package ID to add it to your booking:");
            int packageId = int.Parse(Console.ReadLine() ?? "0");

            string flightClass = "Economy";
            if (_comfortPackageService.ValidatePackageAvailability(flightClass))
            {
                int bookingId = 1;
                _comfortPackageService.AddPackageToBooking(bookingId, packageId);
                Console.WriteLine("Comfort package added to your booking.");
            }
            else
            {
                _comfortPackageUI.ShowValidationErrors("The comfort package is not available for your selected flight class.");
            }
        }

    }

    public void ConfirmBooking(int bookingId)
    {

        BookingSummaryUI bookingSummaryUI = new BookingSummaryUI();
        bookingSummaryUI.RenderUpdatedSummary(bookingId);
    }
}