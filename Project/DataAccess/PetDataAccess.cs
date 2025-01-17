public static class PetDataAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/pets.json"));

    private static readonly GenericJsonAccess<PetModel> _petAccess = new(_filePath);

    public static List<PetModel> LoadPetTypes()
    {
        return _petAccess.LoadAll();
    }

    public static void SavePetBooking(PetModel petDetails, int bookingId)
    {
        var bookings = BookingAccess.LoadAll();
        var booking = bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking != null)
        {
            booking.Pets.Add(petDetails);
            BookingAccess.WriteAll(bookings);
        }
    }

    public static decimal GetPetFees(string petType, string location)
    {
        var petTypes = LoadPetTypes();
        var pet = petTypes.FirstOrDefault(p => p.Type.Equals(petType, StringComparison.OrdinalIgnoreCase));
        return pet?.Fee[location] ?? 0;
    }
}