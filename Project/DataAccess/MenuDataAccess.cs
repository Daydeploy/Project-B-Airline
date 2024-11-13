using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MenuDataAccess
{
    private const string MenuFilePath = "menuOptions.json";

    public List<MenuOption> LoadMenuOptions()
    {
        if (!File.Exists(MenuFilePath))
        {
            return new List<MenuOption>();
        }

        string json = File.ReadAllText(MenuFilePath);
        return JsonSerializer.Deserialize<List<MenuOption>>(json);
    }

    public MenuOption GetMenuItemById(int id)
    {
        var options = LoadMenuOptions();
        return options.Find(option => option.MenuItemID == id);
    }

    public void SaveMenuOptions(List<MenuOption> options)
    {
        string json = JsonSerializer.Serialize(options);
        File.WriteAllText(MenuFilePath, json);
    }
}

public class MenuOption
{
    public int MenuItemID { get; set; }
    public string MenuOptionDescription { get; set; }
    public string Action { get; set; }
}