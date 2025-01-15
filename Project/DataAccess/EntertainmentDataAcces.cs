public class EntertainmentDataAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
            @"DataSources/entertainment.json"));

    private static readonly GenericJsonAccess<EntertainmentModel> _entertainmentAccess = new(_filePath);

    public static List<EntertainmentModel> LoadAll()
    {
        return _entertainmentAccess.LoadAll() ?? new List<EntertainmentModel>();
    }

    public static bool WriteAll(List<EntertainmentModel> entertainments)
    {
        if (entertainments == null)
            return false;

        _entertainmentAccess.WriteAll(entertainments);
        return true;
    }

    public static EntertainmentModel? GetEntertainment(int entertainmentId)
    {
        var entertainmentOptions = LoadAll();
        return entertainmentOptions.Find(option => option.Id == entertainmentId);
    }
}