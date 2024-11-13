using System;

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
        }
        else
        {
            Console.WriteLine($"Seat {seatNumber} is not available.");
        }

        _seatChart.DisplaySeatingChart();
    }
} 