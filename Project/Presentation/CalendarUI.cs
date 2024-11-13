public class CalendarUI
{
    private DateTime currentDate;
    private DateTime? selectedStartDate;
    private DateTime? selectedEndDate;
    private bool isSelectingEndDate;
    private bool isSelectingYear;
    private bool isSelectingMonth;
    private const ConsoleColor SelectedColor = ConsoleColor.Cyan;
    private const ConsoleColor HighlightColor = ConsoleColor.DarkCyan;
    private const ConsoleColor RangeColor = ConsoleColor.DarkGray;

    public CalendarUI()
    {
        currentDate = DateTime.Now;
        selectedStartDate = null;
        selectedEndDate = null;
        isSelectingEndDate = false;
        isSelectingYear = false;
        isSelectingMonth = false;
    }

    public (DateTime startDate, DateTime endDate) SelectDateRange()
    {
        Console.CursorVisible = false;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            if (isSelectingYear)
            {
                DrawYearSelector();
            }
            else if (isSelectingMonth)
            {
                DrawMonthSelector();
            }
            else
            {
                DrawCalendar();
                DrawInstructions();
                DrawDateRangeInfo();
            }

            var key = Console.ReadKey(true);
            if (isSelectingYear)
            {
                HandleYearSelection(key);
            }
            else if (isSelectingMonth)
            {
                HandleMonthSelection(key);
            }
            else
            {
                HandleKeyInput(key);
                switch (key.Key)
                {
                    case ConsoleKey.T:
                        currentDate = DateTime.Now;
                        break;
                    case ConsoleKey.Y:
                        isSelectingYear = true;
                        break;
                    case ConsoleKey.M:
                        isSelectingMonth = true;
                        break;
                    case ConsoleKey.Enter:
                        if (!selectedStartDate.HasValue)
                        {
                            selectedStartDate = currentDate.Date;
                            isSelectingEndDate = true;
                        }
                        else if (!selectedEndDate.HasValue)
                        {
                            if (currentDate.Date >= selectedStartDate.Value)
                            {
                                selectedEndDate = currentDate.Date;
                                done = true;
                            }
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (selectedStartDate.HasValue && !selectedEndDate.HasValue)
                        {
                            selectedStartDate = null;
                            isSelectingEndDate = false;
                        }
                        break;
                }
            }
        }

        Console.CursorVisible = true;
        return (selectedStartDate.Value, selectedEndDate.Value);
    }

    private void DrawCalendar()
    {
        var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        Console.WriteLine($"\n   {currentDate:MMMM yyyy}\n");
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

                var currentDate = new DateTime(this.currentDate.Year, this.currentDate.Month, currentDay);
                bool isSelected = IsDateSelected(currentDate);
                bool isHighlighted = this.currentDate.Date == currentDate;
                bool isInRange = IsDateInRange(currentDate);
                bool isToday = currentDate.Date == DateTime.Now.Date;

                if (isSelected)
                {
                    Console.ForegroundColor = SelectedColor;
                }
                else if (isHighlighted)
                {
                    Console.ForegroundColor = HighlightColor;
                }
                else if (isInRange)
                {
                    Console.ForegroundColor = RangeColor;
                }

                if (isToday)
                {
                    Console.Write("[");
                    Console.Write($"{currentDay,2}");
                    Console.Write("]");
                }
                else
                {
                    Console.Write($"{currentDay,2} ");
                }
                
                Console.ResetColor();

                currentDay++;
            }
            Console.WriteLine();
            if (currentDay > lastDayOfMonth.Day) break;
        }
    }

    private void DrawYearSelector()
    {
        Console.WriteLine("\n Select Year:");
        Console.WriteLine($"\n<< {currentDate.Year} >>");
        Console.WriteLine("\nUse ← → to change year");
        Console.WriteLine("Enter to select, Esc to cancel");
    }

    private void DrawMonthSelector()
    {
        Console.WriteLine("\n Select Month:");
        string[] months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[..^1];
        for (int i = 0; i < months.Length; i++)
        {
            if (i == currentDate.Month - 1)
            {
                Console.ForegroundColor = HighlightColor;
                Console.WriteLine($">> {months[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"   {months[i]}");
            }
        }
        Console.WriteLine("\nUse ↑ ↓ to select month");
        Console.WriteLine("Enter to confirm, Esc to cancel");
    }

    private void HandleYearSelection(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                currentDate = currentDate.AddYears(-1);
                break;
            case ConsoleKey.RightArrow:
                currentDate = currentDate.AddYears(1);
                break;
            case ConsoleKey.Enter:
                isSelectingYear = false;
                break;
            case ConsoleKey.Escape:
                isSelectingYear = false;
                break;
        }
    }

    private void HandleMonthSelection(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                currentDate = currentDate.AddMonths(-1);
                break;
            case ConsoleKey.DownArrow:
                currentDate = currentDate.AddMonths(1);
                break;
            case ConsoleKey.Enter:
                isSelectingMonth = false;
                break;
            case ConsoleKey.Escape:
                isSelectingMonth = false;
                break;
        }
    }

    private void HandleKeyInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.RightArrow:
            case ConsoleKey.D:
                currentDate = currentDate.AddDays(1);
                break;
            case ConsoleKey.LeftArrow:
            case ConsoleKey.A:
                currentDate = currentDate.AddDays(-1);
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                currentDate = currentDate.AddDays(-7);
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                currentDate = currentDate.AddDays(7);
                break;
        }
    }

    private bool IsDateSelected(DateTime date)
    {
        return (selectedStartDate.HasValue && date.Date == selectedStartDate.Value.Date) ||
               (selectedEndDate.HasValue && date.Date == selectedEndDate.Value.Date);
    }

    private bool IsDateInRange(DateTime date)
    {
        if (!selectedStartDate.HasValue || !selectedEndDate.HasValue)
        {
            return selectedStartDate.HasValue && date.Date > selectedStartDate.Value.Date && date.Date <= currentDate.Date;
        }
        return date.Date > selectedStartDate.Value.Date && date.Date < selectedEndDate.Value.Date;
    }

    private void DrawInstructions()
    {
        Console.WriteLine("\nControls:");
        Console.WriteLine("← → or A/D : Change days    T: Today");
        Console.WriteLine("↑ ↓ or W/S : Navigate weeks Y: Year selector");
        Console.WriteLine("Enter      : Select date    M: Month selector");
        Console.WriteLine("Esc        : Clear selection");
        Console.WriteLine();

        if (!selectedStartDate.HasValue)
        {
            Console.WriteLine("Select start date");
        }
        else if (!selectedEndDate.HasValue)
        {
            Console.WriteLine($"Start date: {selectedStartDate.Value:d}");
            Console.WriteLine("Select end date");
        }
    }

    private void DrawDateRangeInfo()
    {
        if (selectedStartDate.HasValue)
        {
            int daysSelected = 0;
            if (selectedEndDate.HasValue)
            {
                daysSelected = (int)(selectedEndDate.Value - selectedStartDate.Value).TotalDays + 1;
            }
            else if (currentDate.Date > selectedStartDate.Value)
            {
                daysSelected = (int)(currentDate.Date - selectedStartDate.Value).TotalDays + 1;
            }

            if (daysSelected > 0)
            {
                Console.WriteLine($"\nSelected range: {daysSelected} day{(daysSelected != 1 ? "s" : "")}");
            }
        }
    }
}