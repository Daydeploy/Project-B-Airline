public class MenuDataAccess
{
    private static string _filePath =
        System.IO.Path.GetFullPath(
            System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/menuOptions.json"));

    private static GenericJsonAccess<MenuOptionModel> _menuAccess = new GenericJsonAccess<MenuOptionModel>(_filePath);


    public static List<MenuOptionModel> LoadAll()
    {
        return _menuAccess.LoadAll();
    }

    public static void WriteAll(List<MenuOptionModel> menuOptions)
    {
        _menuAccess.WriteAll(menuOptions);
    }
}