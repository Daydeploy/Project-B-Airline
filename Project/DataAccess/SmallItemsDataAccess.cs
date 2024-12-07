using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SmallItemsDataAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/smallItems.json"));
    private static List<SmallItem> _smallItems;

    static SmallItemsDataAccess()
    {
        _smallItems = LoadAvailableItems();
    }

    private static List<SmallItem> LoadAvailableItems()
    {
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<SmallItem>>(json);
    }

    public static List<SmallItem> GetAvailableItems()
    {
        return _smallItems;
    }

    public static bool CheckItemAvailability(int itemId)
    {
        var item = _smallItems.Find(i => i.Id == itemId);
        return item != null;
    }

    public static void AddItemToBooking(int itemId, int bookingId)
    {
    }
}

public class SmallItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Availability { get; set; }
} 