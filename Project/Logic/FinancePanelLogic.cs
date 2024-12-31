public class FinancePanelLogic
{
    public class FinancialMetrics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRevenue { get; set; }
        public int BookingCount { get; set; }
        public int AverageBookingValue { get; set; }
    }

    public static FinancialMetrics GetFinancialMetrics(DateTime startDate, DateTime endDate)
    {
        var bookings = BookingAccess.LoadAll()
            .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
            .ToList();

        int totalRevenue = bookings.Sum(b => b.TotalPrice);
        int bookingCount = bookings.Count;

        return new FinancialMetrics
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalRevenue = totalRevenue,
            BookingCount = bookingCount,
            AverageBookingValue = bookingCount > 0 ? totalRevenue / bookingCount : 0
        };
    }

    public static FinancialMetrics ShowYearlyData(int year)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = year == DateTime.Now.Year ? DateTime.Now : new DateTime(year, 1, 12, 31, 23, 59, 59);

        return GetFinancialMetrics(startDate, endDate);
    }

    public static FinancialMetrics ShowMonthlyData(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = year == DateTime.Now.Year && month == DateTime.Now.Month
            ? DateTime.Now
            : startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

        return GetFinancialMetrics(startDate, endDate);
    }

    public static FinancialMetrics ShowDailyData(DateTime date)
    {
        var startDate = date.Date;
        var endDate = date.Date == DateTime.Now.Date
            ? DateTime.Now
            : date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

        return GetFinancialMetrics(startDate, endDate);
    }
}