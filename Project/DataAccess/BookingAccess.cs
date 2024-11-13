static class BookingAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/bookings.json"));
    private static GenericJsonAccess<BookingModel> _bookingAccess = new GenericJsonAccess<BookingModel>(_filePath);

    public static List<BookingModel> LoadAll()
    {
        return _bookingAccess.LoadAll();
    }

    public static void WriteAll(List<BookingModel> bookings)
    {
        _bookingAccess.WriteAll(bookings);
    }
}