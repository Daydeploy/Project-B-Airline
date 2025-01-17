public class FinanceUserLogic
{
    private const int MIN_YEAR = 2024;
    private readonly AccountsLogic _accountsLogic = new();
    IBookingAccess _bookingAccess = new BookingAccess();

    public List<BookingModel> GetPurchasesByYear(int userId, int year)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.UserId == userId && b.BookingDate.Year == year)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetPurchasesByQuarter(int userId, int year, int quarter)
    {
        var (startDate, endDate) = GetQuarterDates(year, quarter);
        return _bookingAccess.LoadAll()
            .Where(b => b.UserId == userId &&
                        b.BookingDate >= startDate &&
                        b.BookingDate <= endDate)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetPurchasesByMonth(int userId, int year, int month)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.UserId == userId &&
                        b.BookingDate.Year == year &&
                        b.BookingDate.Month == month)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetAllBookingsByYear(int year)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.BookingDate.Year == year)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetAllBookingsByQuarter(int year, int quarter)
    {
        var (startDate, endDate) = GetQuarterDates(year, quarter);
        return _bookingAccess.LoadAll()
            .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetAllBookingsByMonth(int year, int month)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.BookingDate.Year == year && b.BookingDate.Month == month)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetAllPurchases(int userId)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToList();
    }

    public List<BookingModel> GetRecentPurchases(int userId, int count = 5)
    {
        return _bookingAccess.LoadAll()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .Take(count)
            .ToList();
    }

    public List<BookingModel> GetRecentPurchasesAdmin(int count = 5)
    {
        return _bookingAccess.LoadAll()
            .OrderByDescending(b => b.BookingDate)
            .Take(count)
            .ToList();
    }

    private (DateTime startDate, DateTime endDate) GetQuarterDates(int year, int quarter)
    {
        var startMonth = (quarter - 1) * 3 + 1;
        var startDate = new DateTime(year, startMonth, 1);
        var endDate = startDate.AddMonths(3).AddDays(-1);
        return (startDate, endDate);
    }

    public bool IsValidYear(int year)
    {
        return year >= MIN_YEAR;
    }

    public (decimal totalSpent, decimal avgPerBooking, decimal mostExpensive, string mostFrequentClass, int bookingCount
        )
        GetSpendingAnalysis(int userId)
    {
        var bookings = GetAllPurchases(userId);

        if (!bookings.Any())
            return (0, 0, 0, string.Empty, 0);

        var totalSpent = bookings.Sum(b => b.TotalPrice);
        var avgPerBooking = bookings.Average(b => (decimal)b.TotalPrice);
        var mostExpensive = bookings.Max(b => b.TotalPrice);
        var mostFrequentClass = bookings
            .SelectMany(b => b.Passengers)
            .GroupBy(p => p.SeatNumber?.Substring(0, 1))
            .OrderByDescending(g => g.Count())
            .First().Key;

        return (totalSpent, avgPerBooking, mostExpensive, mostFrequentClass, bookings.Count);
    }

    public List<AccountModel> GetAllUsers()
    {
        return _accountsLogic.GetAllAccounts()
            .Where(a => !a.EmailAddress.ToLower().Equals("finance"))
            .ToList();
    }
}