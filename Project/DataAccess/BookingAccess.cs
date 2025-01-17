internal static class BookingAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/bookings.json"));

    private static readonly GenericJsonAccess<BookingModel> _bookingAccess = new(_filePath);

    public static List<BookingModel> LoadAll()
    {
        return _bookingAccess.LoadAll();
    }

    public static void WriteAll(List<BookingModel> bookings)
    {
        _bookingAccess.WriteAll(bookings);
    }
}