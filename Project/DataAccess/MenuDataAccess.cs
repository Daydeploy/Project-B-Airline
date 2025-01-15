public class MenuDataAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(
            Path.Combine(Environment.CurrentDirectory, @"DataSources/menuOptions.json"));

    private static readonly GenericJsonAccess<MenuOptionModel> _menuAccess = new(_filePath);


    public static List<MenuOptionModel> LoadAll()
    {
        return _menuAccess.LoadAll();
    }

    public static void WriteAll(List<MenuOptionModel> menuOptions)
    {
        _menuAccess.WriteAll(menuOptions);
    }
}