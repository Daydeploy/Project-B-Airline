using System;
using System.Collections.Generic;

public class BookingProcess
{
    private SeatChart _seatChart;

    public BookingProcess()
    {
        _seatChart = new SeatChart();
    }

    public void StartBooking()
    {
        _seatChart.DisplaySeatingChart();

        Console.WriteLine("Enter the seat number you want to reserve (e.g., 1A):");
        string seatNumber = Console.ReadLine();

        if (_seatChart.ReserveSeat(seatNumber))
        {
            Console.WriteLine($"Seat {seatNumber} has been reserved successfully.");
            
            Console.WriteLine("Do you want to book a pet? (y/n)");
            string petBookingResponse = Console.ReadLine();
            List<PetModel> petsToBook = new List<PetModel>();

            if (petBookingResponse?.ToLower() == "y")
            {
                petsToBook = GetPetDetails();
            }

            var booking = CreateBooking(seatNumber, petsToBook);
            Console.WriteLine("\nFlight booked successfully!\n");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Flight: {selectedFlight.Origin} to {selectedFlight.Destination}");
            Console.WriteLine($"Departure: {selectedFlight.DepartureTime}");
            Console.WriteLine("\nPassengers:");

            foreach (var passenger in booking.Passengers)
            {
                Console.WriteLine($"\nName: {passenger.Name}");
                Console.WriteLine($"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
                Console.WriteLine($"Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
            }

            Console.WriteLine("\nPets:");
            foreach (var pet in booking.Pets)
            {
                Console.WriteLine($"Type: {pet.Type}, Size: {pet.Size}, Location: {pet.SeatingLocation}");
            }

            Console.WriteLine($"\nTotal Price: {booking.TotalPrice} EUR");
        }
        else
        {
            Console.WriteLine("Seat reservation failed.");
        }
    }

    private List<PetModel> GetPetDetails()
    {
        List<PetModel> pets = new List<PetModel>();
        Console.WriteLine("Enter the number of pets you want to book for:");
        int numberOfPets = int.Parse(Console.ReadLine());

        for (int i = 0; i < numberOfPets; i++)
        {
            Console.WriteLine($"Enter details for pet {i + 1}:");
            Console.Write("Type: ");
            string type = Console.ReadLine();
            Console.Write("Size: ");
            string size = Console.ReadLine();
            Console.Write("Seating Location (Seat/Luggage Room): ");
            string seatingLocation = Console.ReadLine();

            pets.Add(new PetModel
            {
                Type = type,
                Size = size,
                SeatingLocation = seatingLocation
            });
        }

        return pets;
    }
} 