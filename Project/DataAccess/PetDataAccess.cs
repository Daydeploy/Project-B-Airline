using System.Collections.Generic;

public static class PetDataAccess
{
    private static string _filePath =
        System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/pets.json"));

    private static GenericJsonAccess<PetModel> _petAccess = new GenericJsonAccess<PetModel>(_filePath);

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