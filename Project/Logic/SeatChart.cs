using System;
using System.Collections.Generic;

public class SeatChart
{
    private Dictionary<string, string> _seats;

    public SeatChart()
    {
        _seats = new Dictionary<string, string>();
        InitializeSeats();
    }

    private void InitializeSeats()
    {
        for (int row = 1; row <= 30; row++)
        {
            for (char col = 'A'; col <= 'F'; col++)
            {
                string seatNumber = $"{row}{col}";
                _seats[seatNumber] = "Available";
            }
        }
    }

    public void DisplaySeatingChart()
    {
        Console.WriteLine("Boeing 737 Seating Chart");
        Console.WriteLine("+---------+----------+");
        Console.WriteLine("| Seat No. | Status   |");
        Console.WriteLine("+---------+----------+");

        for (int row = 1; row <= 30; row++)
        {
            for (char col = 'A'; col <= 'F'; col++)
            {
                string seatNumber = $"{row}{col}";
                string seatStatus = _seats[seatNumber];

                Console.Write($"| {seatNumber,-9} | {seatStatus,-8} |");
                if (col == 'F')
                    Console.WriteLine();
                else
                    Console.Write(" ");
            }
        }

        Console.WriteLine("+---------+----------+");
    }

    public bool ReserveSeat(string seatNumber)
    {
        if (_seats.ContainsKey(seatNumber) && _seats[seatNumber] == "Available")
        {
            _seats[seatNumber] = "Reserved";
            return true;
        }
        return false;
    }
}