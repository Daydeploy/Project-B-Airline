public class CalendarUI
{
    private DateTime currentDate;
    private const ConsoleColor SelectedColor = ConsoleColor.Cyan;
    private const ConsoleColor HighlightColor = ConsoleColor.DarkCyan;
    private const ConsoleColor RedHighlightColor = ConsoleColor.Red; // Highlight selected date in red

    private DateTime? highlightDate; // Date to highlight in red

    public CalendarUI(DateTime? startingDate = null, DateTime? highlightDate = null)
    {
        this.currentDate = startingDate ?? DateTime.Now;
        this.highlightDate = highlightDate;
    }

    public DateTime SelectDate()
    {
        Console.CursorVisible = false;
        DateTime selectedDate = currentDate.Date;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            DrawCalendar(selectedDate);
            DrawInstructions();

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    selectedDate = selectedDate.AddDays(-1);
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    selectedDate = selectedDate.AddDays(1);
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    selectedDate = selectedDate.AddDays(-7);
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    selectedDate = selectedDate.AddDays(7);
                    break;
                case ConsoleKey.T:
                    selectedDate = DateTime.Now.Date;
                    break;
                case ConsoleKey.Y:
                    SelectYear(ref selectedDate);
                    break;
                case ConsoleKey.M:
                    SelectMonth(ref selectedDate);
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    return DateTime.MinValue;
            }
        }

        Console.CursorVisible = true;
        return selectedDate;
    }

    private void DrawCalendar(DateTime selectedDate)
    {
        var firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        Console.WriteLine($"\n   {selectedDate:MMMM yyyy}\n");
        Console.WriteLine(" Su Mo Tu We Th Fr Sa");

        int dayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        int currentDay = 1;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i == 0 && j < dayOfWeek || currentDay > lastDayOfMonth.Day)
                {
                    Console.Write("   ");
                    continue;
                }

                var currentDate = new DateTime(selectedDate.Year, selectedDate.Month, currentDay);
                bool isSelected = selectedDate.Date == currentDate.Date;
                bool isToday = currentDate.Date == DateTime.Now.Date;
                bool isHighlighted = highlightDate.HasValue && currentDate.Date == highlightDate.Value.Date;

                if (isSelected)
                {
                    Console.ForegroundColor = SelectedColor;
                }
                else if (isHighlighted)
                {
                    Console.ForegroundColor = RedHighlightColor;
                }
                else if (isToday)
                {
                    Console.ForegroundColor = HighlightColor;
                }

                Console.Write($"{currentDay,2} ");
                Console.ResetColor();

                currentDay++;
            }

            Console.WriteLine();
            if (currentDay > lastDayOfMonth.Day) break;
        }
    }

    private void SelectYear(ref DateTime selectedDate)
    {
        bool done = false;
        while (!done)
        {
            Console.Clear();
            Console.WriteLine($"\n Select Year: {selectedDate.Year}\n");
            Console.WriteLine("Use ← → to change year, Enter to confirm, Esc to cancel");

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    selectedDate = selectedDate.AddYears(-1);
                    break;
                case ConsoleKey.RightArrow:
                    selectedDate = selectedDate.AddYears(1);
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private void SelectMonth(ref DateTime selectedDate)
    {
        bool done = false;
        while (!done)
        {
            Console.Clear();
            Console.WriteLine($"\n Select Month: {selectedDate:MMMM}\n");
            Console.WriteLine("Use ↑ ↓ to change month, Enter to confirm, Esc to cancel");

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedDate = selectedDate.AddMonths(-1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedDate = selectedDate.AddMonths(1);
                    break;
                case ConsoleKey.Enter:
                    done = true;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private void DrawInstructions()
    {
        Console.WriteLine("\nControls:");
        Console.WriteLine("← → or A/D : Move by day    ↑ ↓ or W/S : Move by week");
        Console.WriteLine("T          : Today          Y          : Year selector");
        Console.WriteLine("M          : Month selector Enter      : Select date");
        Console.WriteLine("Esc        : Cancel");
    }
}
