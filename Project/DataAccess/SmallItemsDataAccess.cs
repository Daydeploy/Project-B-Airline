public class SmallItemsDataAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
            @"DataSources/smallItems.json"));

    private static readonly GenericJsonAccess<SmallItemsModel> _smallItemsModel = new(_filePath);


    public static List<SmallItemsModel> LoadAll()
    {
        return _smallItemsModel.LoadAll();
    }

    public static void WriteAll(List<SmallItemsModel> smallItems)
    {
        _smallItemsModel.WriteAll(smallItems);
    }
}