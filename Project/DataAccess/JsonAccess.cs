public static class AccountsAccess
{
    private static readonly IAccess<AccountModel> _dataAccess = new AccountJsonAccess();

    public static List<AccountModel> LoadAll() => _dataAccess.LoadAll();
    public static void WriteAll(List<AccountModel> accounts) => _dataAccess.WriteAll(accounts);

    private class AccountJsonAccess : BaseJsonAccess<AccountModel>
    {
        public AccountJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/accounts.json")))
        { }
    }
}

public static class AirportAccess
{
    private static readonly IAccess<AirportModel> _dataAccess = new AirportJsonAccess();

    public static List<AirportModel> LoadAllAirports() => _dataAccess.LoadAll();

    public static bool WriteAllAirports(List<AirportModel> airports)
    {
        if (airports == null)
            return false;

        _dataAccess.WriteAll(airports);
        return true;
    }

    public static bool AddAirport(AirportModel newAirport)
    {
        if (newAirport == null)
            return false;

        var airports = LoadAllAirports();
        if (airports.Any(a => a.AirportID == newAirport.AirportID))
            return false;

        airports.Add(newAirport);
        return WriteAllAirports(airports);
    }

    private class AirportJsonAccess : BaseJsonAccess<AirportModel>
    {
        public AirportJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/airports.json")))
        { }
    }
}

public static class BookingAccess
{
    private static readonly IAccess<BookingModel> _dataAccess = new BookingJsonAccess();

    public static List<BookingModel> LoadAll() => _dataAccess.LoadAll();
    public static void WriteAll(List<BookingModel> bookings) => _dataAccess.WriteAll(bookings);

    private class BookingJsonAccess : BaseJsonAccess<BookingModel>
    {
        public BookingJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/bookings.json")))
        { }
    }
}

public static class ComfortPackageDataAccess
{
    private static readonly IAccess<ComfortPackageModel> _dataAccess = new ComfortPackageJsonAccess();

    public static List<ComfortPackageModel> LoadAll() => _dataAccess.LoadAll();

    public static ComfortPackageModel? GetComfortPackage(int packageId)
    {
        var comfortPackageOptions = LoadAll();
        return comfortPackageOptions.Find(option => option.Id == packageId);
    }

    private class ComfortPackageJsonAccess : BaseJsonAccess<ComfortPackageModel>
    {
        public ComfortPackageJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/comfortPackages.json")))
        { }
    }
}

public static class EntertainmentDataAccess
{
    private static readonly IAccess<EntertainmentModel> _dataAccess = new EntertainmentJsonAccess();

    public static List<EntertainmentModel> LoadAll() => _dataAccess.LoadAll();

    public static EntertainmentModel? GetEntertainment(int entertainmentId)
    {
        var entertainmentOptions = LoadAll();
        return entertainmentOptions.Find(option => option.Id == entertainmentId);
    }

    private class EntertainmentJsonAccess : BaseJsonAccess<EntertainmentModel>
    {
        public EntertainmentJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/entertainment.json")))
        { }
    }
}

public static class FlightsAccess
{
    private static readonly IAccess<FlightModel> _dataAccess = new FlightJsonAccess();

    public static List<FlightModel> LoadAll() => _dataAccess.LoadAll();
    public static void WriteAll(List<FlightModel> flights) => _dataAccess.WriteAll(flights);

    private class FlightJsonAccess : BaseJsonAccess<FlightModel>
    {
        public FlightJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/flights.json")))
        { }
    }
}

public static class MenudataAccess
{
    private static readonly IAccess<MenuOptionModel> _dataAccess = new MenuJsonAccess();

    public static List<MenuOptionModel> LoadAll() => _dataAccess.LoadAll();

    public static void WriteAll(List<MenuOptionModel> menus) => _dataAccess.WriteAll(menus);

    private class MenuJsonAccess : BaseJsonAccess<MenuOptionModel>
    {
        public MenuJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/menuOptions.json")))
        { }
    }
}

public static class PetDataAccess
{
    private static readonly IAccess<PetModel> _dataAccess = new PetJsonAccess();

    public static List<PetModel> LoadPetTypes() => _dataAccess.LoadAll();

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
        var pet = petTypes.FirstOrDefault(p =>
            p.Type.Equals(petType, StringComparison.OrdinalIgnoreCase));
        return pet?.Fee[location] ?? 0;
    }

    private class PetJsonAccess : BaseJsonAccess<PetModel>
    {
        public PetJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/pets.json")))
        { }
    }
}

public static class SmallItemsDataAccess
{
    private static readonly IAccess<SmallItemsModel> _dataAccess = new SmallItemJsonAccess();

    public static List<SmallItemsModel> LoadAll() => _dataAccess.LoadAll();

    public static void WriteAll(List<SmallItemsModel> smallItems) => _dataAccess.WriteAll(smallItems);

    private class SmallItemJsonAccess : BaseJsonAccess<SmallItemsModel>
    {
        public SmallItemJsonAccess() : base(
            Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/smallItems.json")))
        { }
    }
}