using System;
using System.Collections.Generic;

public class SeatSelectionUI
{
    private const int ROWS = 30;
    private const int SEATS_PER_ROW = 6;
    private Dictionary<string, bool> occupiedSeats = new Dictionary<string, bool>();
    
    private readonly (int StartRow, int EndRow)[] seatClasses = new[]
    {
        (1, 3),    // First Class
        (4, 8),    // Business Class
        (9, 30)    // Economy Class
    };

    public string SelectSeat(string aircraftType)
    {
        int currentRow = 1;
        int currentSeat = 0;
        bool seatSelected = false;

        while (!seatSelected)
        {
            Console.Clear();
            DisplayLegend();
            DisplayPlane(aircraftType, currentRow, currentSeat);
            
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentRow > 1) currentRow--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentRow < ROWS) currentRow++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentSeat > 0) currentSeat--;
                    break;
                case ConsoleKey.RightArrow:
                    if (currentSeat < SEATS_PER_ROW - 1) currentSeat++;
                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("\nSeat selection cancelled.");
                    return null;
                case ConsoleKey.Enter:
                    string seatNumber = $"{currentRow}{(char)('A' + currentSeat)}";
                    if (!occupiedSeats.ContainsKey(seatNumber))
                    {
                        return seatNumber;
                    }
                    break;
            }
        }

        return null;
    }

    private void DisplayLegend()
    {
        Console.WriteLine("\n === Seat Selection === ");
        Console.WriteLine("Use arrow keys to navigate, Enter to select, ESC to cancel\n");
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("■ First Class ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("■ Business Class ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("■ Economy Class ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("■ Occupied ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("□ Available\n");
        
        Console.Write("     ");
        for (char c = 'A'; c < 'A' + SEATS_PER_ROW; c++)
        {
            Console.Write($" {c} ");
            if (c == 'C')
                Console.Write(" ");
        }
        Console.WriteLine("\n");
    }

    private void DisplayPlane(string aircraftType, int selectedRow, int selectedSeat)
    {
        switch (aircraftType)
        {
            case "Boeing 737":
                Display737(selectedRow, selectedSeat);
                break;
            case "Boeing 787":
                Display787(selectedRow, selectedSeat);
                break;
            case "Airbus A330":
                DisplayA330(selectedRow, selectedSeat);
                break;
        }
    }

    private void Display737(int selectedRow, int selectedSeat)
    {
        for (int row = 1; row <= ROWS; row++)
        {
            Console.Write($" {row,2} |");
            
            for (int seat = 0; seat < SEATS_PER_ROW; seat++)
            {
                string seatNumber = $"{row}{(char)('A' + seat)}";
                bool isSelected = row == selectedRow && seat == selectedSeat;
                bool isOccupied = occupiedSeats.ContainsKey(seatNumber);

                // color based on seat class
                if (row <= seatClasses[0].EndRow)
                    Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
                else if (row <= seatClasses[1].EndRow)
                    Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

                
                if (isOccupied)
                    Console.ForegroundColor = ConsoleColor.Red;

                
                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("[■]");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write(isOccupied ? " ■ " : " □ ");
                }

                // Add aisle space after seats C and D
                if (seat == 2)
                    Console.Write(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |");

            // Add space between classes
            if (row == seatClasses[0].EndRow || row == seatClasses[1].EndRow)
                Console.WriteLine("     +" + new string('-', SEATS_PER_ROW * 3 + 1) + "+");
        }
    }

    private void Display787(int selectedRow, int selectedSeat)
    {
        // Boeing 787 layout
        for (int row = 1; row <= ROWS; row++)
        {
            Console.Write($" {row,2} |");
            
            for (int seat = 0; seat < SEATS_PER_ROW; seat++)
            {
                string seatNumber = $"{row}{(char)('A' + seat)}";
                bool isSelected = row == selectedRow && seat == selectedSeat;
                bool isOccupied = occupiedSeats.ContainsKey(seatNumber);

                // color based on seat class
                if (row <= 6)
                    Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
                else if (row <= 8)
                    Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

                
                if (isOccupied)
                    Console.ForegroundColor = ConsoleColor.Red;

                
                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("[■]");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write(isOccupied ? " ■ " : " □ ");
                }

                // Add aisle space after seats E and F
                if (seat == 4)
                    Console.Write(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |");

            // Add space between classes
            if (row == 6 || row == 8)
                Console.WriteLine("     +" + new string('-', SEATS_PER_ROW * 3 + 1) + "+");
        }
    }

    private void DisplayA330(int selectedRow, int selectedSeat)
    {
        // Airbus 330 layout
        for (int row = 1; row <= ROWS; row++)
        {
            Console.Write($" {row,2} |");
            
            for (int seat = 0; seat < SEATS_PER_ROW; seat++)
            {
                string seatNumber = $"{row}{(char)('A' + seat)}";
                bool isSelected = row == selectedRow && seat == selectedSeat;
                bool isOccupied = occupiedSeats.ContainsKey(seatNumber);

                // color based on seat class
                if (row <= 3)
                    Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
                else if (row <= 8)
                    Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

                
                if (isOccupied)
                    Console.ForegroundColor = ConsoleColor.Red;

                
                if (isSelected)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("[■]");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write(isOccupied ? " ■ " : " □ ");
                }

                // Add aisle space after seats C and D
                if (seat == 2)
                    Console.Write(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" |");

            // Add space between classes
            if (row == 3 || row == 8)
                Console.WriteLine("     +" + new string('-', SEATS_PER_ROW * 3 + 1) + "+");
        }
    }

    public void SetSeatOccupied(string seatNumber, bool occupied = true)
    {
        if (occupied)
            occupiedSeats[seatNumber] = true;
        else
            occupiedSeats.Remove(seatNumber);
    }

    public string GetSeatClass(string seatNumber)
    {
        int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));
        
        if (row <= seatClasses[0].EndRow)
            return "First";
        if (row <= seatClasses[1].EndRow)
            return "Business";
        return "Economy";
    }
}
// using System;
// using System.Collections.Generic;

// public class SeatSelectionUI
// {
//     private const int ROWS = 30;
//     private const int SEATS_PER_ROW = 6;
//     private Dictionary<string, bool> occupiedSeats = new Dictionary<string, bool>();
    
//     private readonly (int StartRow, int EndRow)[] seatClasses = new[]
//     {
//         (1, 3),    // First Class
//         (4, 8),    // Business Class
//         (9, 30)    // Economy Class
//     };

//     public string SelectSeat()
//     {
//         int currentRow = 1;
//         int currentSeat = 0;
//         bool seatSelected = false;

//         while (!seatSelected)
//         {
//             Console.Clear();
//             DisplayLegend();
//             DisplayPlane(currentRow, currentSeat);
            
//             ConsoleKeyInfo key = Console.ReadKey(true);
//             switch (key.Key)
//             {
//                 case ConsoleKey.UpArrow:
//                     if (currentRow > 1) currentRow--;
//                     break;
//                 case ConsoleKey.DownArrow:
//                     if (currentRow < ROWS) currentRow++;
//                     break;
//                 case ConsoleKey.LeftArrow:
//                     if (currentSeat > 0) currentSeat--;
//                     break;
//                 case ConsoleKey.RightArrow:
//                     if (currentSeat < SEATS_PER_ROW - 1) currentSeat++;
//                     break;
//                 case ConsoleKey.Enter:
//                     string seatNumber = $"{currentRow}{(char)('A' + currentSeat)}";
//                     if (!occupiedSeats.ContainsKey(seatNumber))
//                     {
//                         return seatNumber;
//                     }
//                     break;
//             }
//         }

//         return null;
//     }

//     private void DisplayLegend()
//     {
//         Console.WriteLine("\n === Seat Selection === ");
//         Console.WriteLine("Use arrow keys to navigate, Enter to select\n");
        
//         Console.ForegroundColor = ConsoleColor.Magenta;
//         Console.Write("■ First Class ");
//         Console.ForegroundColor = ConsoleColor.Yellow;
//         Console.Write("■ Business Class ");
//         Console.ForegroundColor = ConsoleColor.Cyan;
//         Console.Write("■ Economy Class ");
//         Console.ForegroundColor = ConsoleColor.Red;
//         Console.Write("■ Occupied ");
//         Console.ForegroundColor = ConsoleColor.White;
//         Console.WriteLine("□ Available\n");
        
//         Console.Write("     ");
//         for (char c = 'A'; c < 'A' + SEATS_PER_ROW; c++)
//         {
//             Console.Write($" {c} ");
//         }
//         Console.WriteLine("\n");
//     }

//     private void DisplayPlane(int selectedRow, int selectedSeat)
//     {
//         for (int row = 1; row <= ROWS; row++)
//         {
//             Console.Write($" {row,2} |");
            
//             for (int seat = 0; seat < SEATS_PER_ROW; seat++)
//             {
//                 string seatNumber = $"{row}{(char)('A' + seat)}";
//                 bool isSelected = row == selectedRow && seat == selectedSeat;
//                 bool isOccupied = occupiedSeats.ContainsKey(seatNumber);

//                 // Set color based on seat class
//                 if (row <= seatClasses[0].EndRow)
//                     Console.ForegroundColor = ConsoleColor.Magenta;  // First Class
//                 else if (row <= seatClasses[1].EndRow)
//                     Console.ForegroundColor = ConsoleColor.Yellow;   // Business Class
//                 else
//                     Console.ForegroundColor = ConsoleColor.Cyan;     // Economy Class

//                 // Override color if seat is occupied
//                 if (isOccupied)
//                     Console.ForegroundColor = ConsoleColor.Red;

//                 // Display seat
//                 if (isSelected)
//                 {
//                     Console.BackgroundColor = ConsoleColor.DarkGray;
//                     Console.Write("[■]");
//                     Console.BackgroundColor = ConsoleColor.Black;
//                 }
//                 else
//                 {
//                     Console.Write(isOccupied ? " ■ " : " □ ");
//                 }

//                 // Add aisle space after seats C and D
//                 if (seat == 2)
//                     Console.Write(" ");
//             }
            
//             Console.ForegroundColor = ConsoleColor.White;
//             Console.WriteLine(" |");

//             // Add space between classes
//             if (row == seatClasses[0].EndRow || row == seatClasses[1].EndRow)
//                 Console.WriteLine("     +" + new string('-', SEATS_PER_ROW * 3 + 1) + "+");
//         }
//     }

//     // Method to mark a seat as occupied
//     public void SetSeatOccupied(string seatNumber, bool occupied = true)
//     {
//         if (occupied)
//             occupiedSeats[seatNumber] = true;
//         else
//             occupiedSeats.Remove(seatNumber);
//     }

//     // Method to check if a seat is within a specific class
//     public string GetSeatClass(string seatNumber)
//     {
//         int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));
        
//         if (row <= seatClasses[0].EndRow)
//             return "First";
//         if (row <= seatClasses[1].EndRow)
//             return "Business";
//         return "Economy";
//     }
// }