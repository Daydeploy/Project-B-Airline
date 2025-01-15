public class CalendarUI
{
    private DateTime currentDate;
    private const ConsoleColor SelectedColor = ConsoleColor.Cyan;
    private const ConsoleColor HighlightColor = ConsoleColor.DarkCyan;
    private const ConsoleColor RedHighlightColor = ConsoleColor.Red;
    private const ConsoleColor RangeStartColor = ConsoleColor.Green;
    private const ConsoleColor RangeEndColor = ConsoleColor.Yellow;
    private const ConsoleColor RangePreviewColor = ConsoleColor.DarkGray;

    private DateTime? highlightDate;
    private DateTime? rangeStart;
    private DateTime? rangeEnd;
    private bool isRangeSelection;

    public CalendarUI(DateTime? startingDate = null, DateTime? highlightDate = null, bool isRangeSelection = false)
    {
        this.currentDate = startingDate ?? DateTime.Now;
        this.highlightDate = highlightDate;
        this.isRangeSelection = isRangeSelection;
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
            DrawInstructions(false);

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

    public (DateTime startDate, DateTime endDate) SelectDateRange()
    {
        Console.CursorVisible = false;
        DateTime selectedDate = currentDate.Date;
        rangeStart = null;
        rangeEnd = null;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            DrawCalendar(selectedDate);
            DrawInstructions(true);
            DrawRangeInfo(selectedDate);

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
                    if (!rangeStart.HasValue)
                    {
                        rangeStart = selectedDate;
                    }
                    else if (!rangeEnd.HasValue && selectedDate >= rangeStart.Value)
                    {
                        rangeEnd = selectedDate;
                        done = true;
                    }

                    break;
                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    return (DateTime.MinValue, DateTime.MinValue);
            }
        }

        Console.CursorVisible = true;
        return (rangeStart.Value, rangeEnd.Value);
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
                bool isRangeStartDate = rangeStart.HasValue && currentDate.Date == rangeStart.Value.Date;
                bool isInPreviewRange = rangeStart.HasValue && !rangeEnd.HasValue &&
                                        currentDate.Date >= rangeStart.Value.Date &&
                                        currentDate.Date <= selectedDate.Date;
                bool isInFinalRange = rangeStart.HasValue && rangeEnd.HasValue &&
                                      currentDate.Date >= rangeStart.Value.Date &&
                                      currentDate.Date <= rangeEnd.Value.Date;

                if (isRangeStartDate)
                {
                    Console.ForegroundColor = RangeStartColor;
                }
                else if (isSelected && rangeStart.HasValue && !rangeEnd.HasValue)
                {
                    Console.ForegroundColor = RangeEndColor;
                }
                else if (isSelected)
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
                else if (isInPreviewRange || isInFinalRange)
                {
                    Console.ForegroundColor = RangePreviewColor;
                }

                Console.Write($"{currentDay,2} ");
                Console.ResetColor();

                currentDay++;
            }

            Console.WriteLine();
            if (currentDay > lastDayOfMonth.Day) break;
        }
    }

    private void DrawRangeInfo(DateTime selectedDate)
    {
        if (!isRangeSelection) return;

        if (rangeStart.HasValue)
        {
            Console.WriteLine($"\nRange Start: {rangeStart.Value:d}");
            if (!rangeEnd.HasValue)
            {
                Console.WriteLine($"Current End: {selectedDate:d}");
                var days = (selectedDate - rangeStart.Value).Days;
                Console.WriteLine($"Range Length: {days + 1} days");
            }
        }
    }

    private void DrawInstructions(bool isRangeSelection)
    {
        Console.WriteLine("\nControls:");
        Console.WriteLine("← → or A/D : Move by day    ↑ ↓ or W/S : Move by week");
        Console.WriteLine("T          : Today          Y          : Year selector");
        Console.WriteLine("M          : Month selector Enter      : Select date");
        if (isRangeSelection)
        {
            if (!rangeStart.HasValue)
            {
                Console.WriteLine("Enter      : Select start date");
            }
            else if (!rangeEnd.HasValue)
            {
                Console.WriteLine("Enter      : Confirm end date");
            }
        }

        Console.WriteLine("Esc        : Cancel");
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
}