public class ComfortPackageDataAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
            @"DataSources/comfortPackages.json"));

    private static readonly GenericJsonAccess<ComfortPackageModel> _comfortPackages = new(_filePath);

    public static ComfortPackageModel? GetComfortPackage(int packageId)
    {
        var comfortPackageOptions = LoadAll();
        return comfortPackageOptions.Find(option => option.Id == packageId);
    }

    public static List<ComfortPackageModel> LoadAll()
    {
        return _comfortPackages.LoadAll();
    }
}