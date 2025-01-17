public class PetDataAccess
{
    private readonly IBookingAccess _bookingAccess = new BookingAccess();

    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/pets.json"));

    private readonly GenericJsonAccess<PetModel> _petAccess = new(_filePath);

    public List<PetModel> LoadPetTypes()
    {
        return _petAccess.LoadAll();
    }

    public void SavePetBooking(PetModel petDetails, int bookingId)
    {
        var bookings = _bookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking != null)
        {
            booking.Pets.Add(petDetails);
            _bookingAccess.WriteAll(bookings);
        }
    }

    public decimal GetPetFees(string petType, string location)
    {
        var petTypes = LoadPetTypes();
        var pet = petTypes.FirstOrDefault(p => p.Type.Equals(petType, StringComparison.OrdinalIgnoreCase));
        return pet?.Fee[location] ?? 0;
    }
}