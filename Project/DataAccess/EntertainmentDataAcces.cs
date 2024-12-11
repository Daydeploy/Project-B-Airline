public class EntertainmentDataAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/entertainment.json"));
    private static GenericJsonAccess<EntertainmentModel> _entertainmentAccess = new GenericJsonAccess<EntertainmentModel>(_filePath);

    public static List<EntertainmentModel> LoadAll()
    {
        return _entertainmentAccess.LoadAll();
    }

    public static void WriteAll(List<EntertainmentModel> entertainments)
    {
        _entertainmentAccess.WriteAll(entertainments);
    }

    public static EntertainmentModel? GetEntertainment(int entertainmentId)
    {
        var entertainmentOptions = LoadAll();
        return entertainmentOptions.Find(option => option.Id == entertainmentId);
    }
}